import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgIf } from '@angular/common';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgIf],
  template: `
    <section class="mx-auto max-w-md rounded-2xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
      <h1 class="text-2xl font-bold text-slate-900">Login</h1>
      <p class="mt-2 text-sm text-slate-600">Use the demo credentials or register a new user.</p>

      <form class="mt-6 space-y-4" [formGroup]="form" (ngSubmit)="submit()">
        <div>
          <label class="text-sm font-semibold text-slate-700" for="email">Email</label>
          <input id="email" type="email" formControlName="email" class="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-slate-900" />
        </div>

        <div>
          <label class="text-sm font-semibold text-slate-700" for="password">Password</label>
          <input id="password" type="password" formControlName="password" class="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-slate-900" />
        </div>

        <div *ngIf="error" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{{ error }}</div>

        <button type="submit" [disabled]="form.invalid || isSubmitting" class="w-full rounded-lg bg-slate-900 px-4 py-2 font-semibold text-white hover:bg-slate-700">
          {{ isSubmitting ? 'Signing in...' : 'Sign in' }}
        </button>
      </form>

      <div class="mt-4 rounded-lg bg-slate-50 p-3 text-sm text-slate-600">
        <strong>Demo:</strong> demo&#64;taskmanager.com / Demo&#64;123456
      </div>

      <p class="mt-4 text-center text-sm text-slate-600">
        No account?
        <a routerLink="/register" class="font-semibold text-slate-900 hover:underline">Create one</a>
      </p>
    </section>
  `
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected isSubmitting = false;
  protected error = '';

  protected readonly form = this.fb.nonNullable.group({
    email: ['demo@taskmanager.com', [Validators.required, Validators.email]],
    password: ['Demo@123456', [Validators.required, Validators.minLength(8)]]
  });

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error = '';
    this.isSubmitting = true;

    this.auth.login(this.form.getRawValue())
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => this.router.navigateByUrl('/tasks'),
        error: () => (this.error = 'Invalid email or password.')
      });
  }
}
