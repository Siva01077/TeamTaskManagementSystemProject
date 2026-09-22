import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TaskItem, CommentItem } from '../../models/task.model';
import { TaskService } from '../../core/services/task.service';
import { CommentService } from '../../core/services/comment.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  tasks: TaskItem[] = [];
  selectedStatus = '';
  selectedPriority = '';

  comments: { [taskId: number]: CommentItem[] } = {};
  commentText: { [taskId: number]: string } = {};

  constructor(
    private taskService: TaskService,
    private commentService: CommentService
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks(this.selectedStatus, this.selectedPriority).subscribe({
      next: (tasks) => {
        this.tasks = tasks;
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

  loadComments(taskId: number): void {
    this.commentService.getComments(taskId).subscribe({
      next: (comments) => {
        this.comments[taskId] = comments;
      }
    });
  }

  addComment(taskId: number): void {
    const content = this.commentText[taskId];

    if (!content?.trim()) {
      return;
    }

    this.commentService.addComment(taskId, content).subscribe({
      next: () => {
        this.commentText[taskId] = '';
        this.loadComments(taskId);
      }
    });
  }
}
