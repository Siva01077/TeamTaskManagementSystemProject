import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { User } from '../../models/user.model';
import { UserService } from '../../core/services/user.service';
import { TeamService } from '../../core/services/team.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit {
  users: User[] = [];
  newTeamName = '';

  constructor(
    private userService: UserService,
    private teamService: TeamService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
      }
    });
  }

  changeRole(user: User, role: string): void {
    this.userService.changeRole(user.id, role).subscribe({
      next: () => {
        user.role = role as 'Admin' | 'Manager' | 'User';
      }
    });
  }

  createTeam(): void {
    if (!this.newTeamName.trim()) {
      return;
    }

    this.teamService.createTeam(this.newTeamName).subscribe({
      next: () => {
        this.newTeamName = '';
        alert('Team created successfully.');
      }
    });
  }

  deleteUser(user: User): void {
    if (!confirm(`Delete ${user.fullName}?`)) {
      return;
    }

    this.userService.deleteUser(user.id).subscribe({
      next: () => {
        this.loadUsers();
      }
    });
  }
}
