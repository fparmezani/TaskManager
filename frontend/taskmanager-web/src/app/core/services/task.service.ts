import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ChangeTaskStatusRequest, CreateTaskRequest, TaskItem, UpdateTaskRequest } from '../models/task.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/tasks`;

  getAll(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(this.baseUrl);
  }

  create(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.baseUrl, request);
  }

  update(id: string, request: UpdateTaskRequest): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.baseUrl}/${id}`, request);
  }

  changeStatus(id: string, request: ChangeTaskStatusRequest): Observable<TaskItem> {
    return this.http.patch<TaskItem>(`${this.baseUrl}/${id}/status`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
