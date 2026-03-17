import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  name = '';
  email = '';
  password = '';
  errorMessage = '';
  loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onRegister(): void {
    this.loading = true;
    this.errorMessage = '';
    this.authService.register({
      name: this.name,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => this.router.navigate(['/books']),
      error: (err) => {
        this.errorMessage = err.error || 'Registration failed.';
        this.loading = false;
      }
    });
  }
}