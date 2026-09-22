import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TaskItem } from '../../models/task.model';
import { User } from '../../models/user.model';
import { TaskService } from '../../core/services/task.service';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-manager',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manager.component.html',
  styleUrl: './manager.component.css'
})
export class ManagerComponent implements OnInit {
  tasks: TaskItem[] = [];
  users: User[] = [];
  selectedStatus = '';
  selectedPriority = '';

  newTask = {
    title: '',
    description: '',
    deadline: '',
    priority: 'Medium',
    assignedToUserId: 0
  };

  constructor(
    private taskService: TaskService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.loadTasks();
    this.loadUsers();
  }

  loadTasks(): void {
    this.taskService.getTasks(this.selectedStatus, this.selectedPriority).subscribe({
      next: (tasks) => {
        this.tasks = tasks;
      }
    });
  }

  loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users.filter((user) => user.role === 'User');
      }
    });
  }

  createTask(): void {
    if (!this.newTask.title || !this.newTask.deadline || !this.newTask.assignedToUserId) {
      alert('Please fill all required fields.');
      return;
    }

    this.taskService.createTask(this.newTask).subscribe({
      next: () => {
        alert('Task created successfully.');

        this.newTask = {
          title: '',
          description: '',
          deadline: '',
          priority: 'Medium',
          assignedToUserId: 0
        };

        this.loadTasks();
      }
    });
  }

  updateStatus(task: TaskItem, status: string): void {
    this.taskService.updateStatus(task.id, status).subscribe({
      next: () => {
        task.status = status as 'To Do' | 'In Progress' | 'Done';
      }
    });
  }
}
