import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../api.config';
import { CommentItem } from '../../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private readonly apiUrl = `${API_BASE_URL}/Comments`;

  constructor(private http: HttpClient) {}

  getComments(taskId: number): Observable<CommentItem[]> {
    return this.http.get<CommentItem[]>(`${this.apiUrl}/task/${taskId}`);
  }

  addComment(taskId: number, content: string): Observable<CommentItem> {
    return this.http.post<CommentItem>(this.apiUrl, { taskId, content });
  }
}
