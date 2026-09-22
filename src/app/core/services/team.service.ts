import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../api.config';
import { Team, User } from '../../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  private readonly apiUrl = `${API_BASE_URL}/Teams`;

  constructor(private http: HttpClient) {}

  getTeams(): Observable<Team[]> {
    return this.http.get<Team[]>(this.apiUrl);
  }

  getTeamMembers(teamId: number): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/${teamId}/members`);
  }

  createTeam(name: string): Observable<Team> {
    return this.http.post<Team>(this.apiUrl, { name });
  }

  addMember(teamId: number, userId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${teamId}/members/${userId}`, {});
  }

  removeMember(teamId: number, userId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${teamId}/members/${userId}`);
  }
}
