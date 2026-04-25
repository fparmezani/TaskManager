import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgIf } from '@angular/common';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgIf],
  template: `
    <section class="mx-auto max-w-md rounded-2xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
      <h1 class="text-2xl font-bold text-slate-900">Create account</h1>
      <p class="mt-2 text-sm text-slate-600">Register a new user to manage personal tasks.</p>

      <form class="mt-6 space-y-4" [formGroup]="form" (ngSubmit)="submit()">
        <div>
          <label class="text-sm font-semibold text-slate-700" for="email">Email</label>
          <input id="email" type="email" formControlName="email" class="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-slate-900" />
        </div>

        <div>
          <label class="text-sm font-semibold text-slate-700" for="password">Password</label>
          <input id="password" type="password" formControlName="password" class="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-slate-900" />
          <p class="mt-1 text-xs text-slate-500">Minimum 8 characters.</p>
        </div>

        <div *ngIf="error" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{{ error }}</div>

        <button type="submit" [disabled]="form.invalid || isSubmitting" class="w-full rounded-lg bg-slate-900 px-4 py-2 font-semibold text-white hover:bg-slate-700">
          {{ isSubmitting ? 'Creating...' : 'Create account' }}
        </button>
      </form>

      <p class="mt-4 text-center text-sm text-slate-600">
        Already have an account?
        <a routerLink="/login" class="font-semibold text-slate-900 hover:underline">Login</a>
      </p>
    </section>
  `
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected isSubmitting = false;
  protected error = '';

  protected readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error = '';
    this.isSubmitting = true;

    this.auth.register(this.form.getRawValue())
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => this.router.navigateByUrl('/tasks'),
        error: () => (this.error = 'Could not create account. The email may already be registered.')
      });
  }
}
