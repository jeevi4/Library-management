export interface User {
  userId: number;
  name: string;
  email: string;
  role: string;
  token: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}