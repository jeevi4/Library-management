using LibraryPortal.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Database ──────────────────────────────────────────────
// Registers AppDbContext with SQLite using the connection string
// from appsettings.json. Without this, no controller can access
// the database.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString))
        .ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore
                .Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// ── 2. JWT Authentication ────────────────────────────────────
// Reads JWT settings from appsettings.json and configures the
// middleware to validate every incoming token automatically.
// Without this, [Authorize] attribute does nothing.
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

// ── 3. Authorization ─────────────────────────────────────────
// Enables role-based access. Without this, [Authorize(Roles="Admin")]
// won't work even if JWT is set up correctly.
builder.Services.AddAuthorization();

// ── 4. CORS ──────────────────────────────────────────────────
// Allows Angular (port 4200) to call this API (port 5000).
// Without this, every Angular HTTP request gets blocked by
// the browser with a CORS error.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── 5. Controllers + Swagger ─────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configured to accept JWT tokens for testing
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library Portal API",
        Version = "v1"
    });

    // This adds the "Authorize" button in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {your token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── 6. Auto-apply migrations on startup ──────────────────────
// This creates the database and tables automatically when the
// app starts. Without this you'd have to run migrations manually
// every time on a new machine.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── 7. Middleware pipeline ────────────────────────────────────
// ORDER MATTERS — if you put Authorization before Authentication
// it will always return 401. This exact order is required.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseAuthentication();  // ← must come before UseAuthorization
app.UseAuthorization();
app.MapControllers();

app.Run();