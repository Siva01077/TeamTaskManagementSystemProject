import { Routes } from '@angular/router';

import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    loadComponent: () =>
      import('./pages/admin/admin.component').then((m) => m.AdminComponent),
    canActivate: [authGuard, roleGuard(['Admin'])]
  },
  {
    path: 'manager',
    loadComponent: () =>
      import('./pages/manager/manager.component').then((m) => m.ManagerComponent),
    canActivate: [authGuard, roleGuard(['Manager'])]
  },
  {
    path: 'user',
    loadComponent: () =>
      import('./pages/user/user.component').then((m) => m.UserComponent),
    canActivate: [authGuard, roleGuard(['User'])]
  },
  { path: '**', redirectTo: 'dashboard' }
];
