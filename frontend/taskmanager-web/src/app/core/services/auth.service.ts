import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.models';

const STORAGE_KEY = 'taskmanager.auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly currentUserSubject = new BehaviorSubject<AuthResponse | null>(this.loadUser());

  readonly currentUser$ = this.currentUserSubject.asObservable();

  get token(): string | null {
    return this.currentUserSubject.value?.accessToken ?? null;
  }

  get isAuthenticated(): boolean {
    return Boolean(this.token);
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiBaseUrl}/auth/login`, request).pipe(
      tap((response) => this.setCurrentUser(response))
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiBaseUrl}/auth/register`, request).pipe(
      tap((response) => this.setCurrentUser(response))
    );
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.currentUserSubject.next(null);
  }

  private setCurrentUser(response: AuthResponse): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
    this.currentUserSubject.next(response);
  }

  private loadUser(): AuthResponse | null {
    const raw = localStorage.getItem(STORAGE_KEY);

    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as AuthResponse;
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }
}
