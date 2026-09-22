import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  fullName = '';
  email = '';
  password = '';
  role = 'User';

  errorMessage = '';
  successMessage = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.fullName || !this.email || !this.password) {
      this.errorMessage = 'All fields are required.';
      return;
    }

    this.loading = true;

    this.authService
      .register({
        fullName: this.fullName,
        email: this.email,
        password: this.password,
        role: this.role
      })
      .subscribe({
        next: () => {
          this.loading = false;
          this.successMessage = 'Registration successful.';

          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 1000);
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error.error?.message ?? 'Registration failed.';
        }
      });
  }
}
