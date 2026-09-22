import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  get user() {
    return this.authService.getCurrentUser();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  openAdmin(): void {
    this.router.navigate(['/admin']);
  }

  openManager(): void {
    this.router.navigate(['/manager']);
  }

  openUser(): void {
    this.router.navigate(['/user']);
  }
}
