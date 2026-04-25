import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AsyncPipe, NgIf } from '@angular/common';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, AsyncPipe, NgIf],
  template: `
    <header class="border-b border-slate-200 bg-white">
      <nav class="mx-auto flex max-w-6xl items-center justify-between px-4 py-4">
        <a routerLink="/tasks" class="text-xl font-bold text-slate-900">TaskManager</a>

        <div class="flex items-center gap-3">
          <ng-container *ngIf="auth.currentUser$ | async as user; else guestLinks">
            <span class="hidden text-sm text-slate-600 sm:inline">{{ user.email }}</span>
            <button
              type="button"
              class="rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-700"
              (click)="logout()">
              Logout
            </button>
          </ng-container>

          <ng-template #guestLinks>
            <a routerLink="/login" class="text-sm font-semibold text-slate-700 hover:text-slate-950">Login</a>
            <a routerLink="/register" class="rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-700">Register</a>
          </ng-template>
        </div>
      </nav>
    </header>

    <main class="mx-auto max-w-6xl px-4 py-8">
      <router-outlet />
    </main>
  `
})
export class AppComponent {
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
