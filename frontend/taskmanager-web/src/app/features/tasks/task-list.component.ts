import { Component, OnInit, inject } from '@angular/core';
import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { TaskService } from '../../core/services/task.service';
import { CreateTaskRequest, TaskItem, TaskStatus } from '../../core/models/task.models';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [ReactiveFormsModule, NgFor, NgIf, NgClass, DatePipe],
  template: `
    <section class="grid gap-6 lg:grid-cols-[380px_1fr]">
      <aside class="rounded-2xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
        <h1 class="text-2xl font-bold text-slate-900">Tasks</h1>
        <p class="mt-2 text-sm text-slate-600">Create, update, complete, and delete your personal tasks.</p>

        <form class="mt-6 space-y-4" [formGroup]="form" (ngSubmit)="save()">
          <div>
            <label class="text-sm font-semibold text-slate-700" for="title">Title</label>
            <input id="title" formControlName="title" class="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-slate-900" />
          </div>

          <div>
            <label class="text-sm font-semibold text-slate-700" for="description">Description</label>
            <textarea id="description" formControlName="description" rows="4" class="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-slate-900"></textarea>
          </div>

          <div>
            <label class="text-sm font-semibold text-slate-700" for="dueDate">Due date</label>
            <input id="dueDate" type="date" formControlName="dueDate" class="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-slate-900" />
          </div>

          <div *ngIf="error" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{{ error }}</div>

          <div class="flex gap-3">
            <button type="submit" [disabled]="form.invalid || isSaving" class="rounded-lg bg-slate-900 px-4 py-2 font-semibold text-white hover:bg-slate-700">
              {{ selectedTask ? 'Update task' : 'Create task' }}
            </button>
            <button type="button" *ngIf="selectedTask" class="rounded-lg border border-slate-300 px-4 py-2 font-semibold text-slate-700 hover:bg-slate-50" (click)="resetForm()">
              Cancel
            </button>
          </div>
        </form>
      </aside>

      <div class="rounded-2xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
        <div class="flex items-center justify-between gap-4">
          <div>
            <h2 class="text-xl font-bold text-slate-900">My tasks</h2>
            <p class="text-sm text-slate-600">{{ tasks.length }} item(s)</p>
          </div>
          <button type="button" class="rounded-lg border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50" (click)="loadTasks()">
            Refresh
          </button>
        </div>

        <div *ngIf="isLoading" class="mt-8 text-center text-slate-500">Loading tasks...</div>
        <div *ngIf="!isLoading && tasks.length === 0" class="mt-8 rounded-xl border border-dashed border-slate-300 p-8 text-center text-slate-500">
          No tasks yet. Create your first task.
        </div>

        <div class="mt-6 grid gap-4">
          <article *ngFor="let task of tasks" class="rounded-xl border border-slate-200 p-4">
            <div class="flex flex-col justify-between gap-3 sm:flex-row sm:items-start">
              <div>
                <div class="flex flex-wrap items-center gap-2">
                  <h3 class="font-bold text-slate-900">{{ task.title }}</h3>
                  <span class="rounded-full px-2.5 py-1 text-xs font-semibold" [ngClass]="statusClass(task.status)">
                    {{ statusLabel(task.status) }}
                  </span>
                </div>
                <p class="mt-2 text-sm text-slate-600">{{ task.description }}</p>
                <p class="mt-3 text-xs text-slate-500">Due: {{ task.dueDate | date:'mediumDate' }}</p>
              </div>

              <div class="flex flex-wrap gap-2">
                <button type="button" class="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-semibold text-slate-700 hover:bg-slate-50" (click)="edit(task)">
                  Edit
                </button>
                <button type="button" class="rounded-lg border border-emerald-300 px-3 py-1.5 text-sm font-semibold text-emerald-700 hover:bg-emerald-50" (click)="complete(task)" [disabled]="task.status === TaskStatus.Completed">
                  Complete
                </button>
                <button type="button" class="rounded-lg border border-red-300 px-3 py-1.5 text-sm font-semibold text-red-700 hover:bg-red-50" (click)="delete(task)">
                  Delete
                </button>
              </div>
            </div>
          </article>
        </div>
      </div>
    </section>
  `
})
export class TaskListComponent implements OnInit {
  protected readonly TaskStatus = TaskStatus;

  private readonly taskService = inject(TaskService);
  private readonly fb = inject(FormBuilder);

  protected tasks: TaskItem[] = [];
  protected selectedTask: TaskItem | null = null;
  protected isLoading = false;
  protected isSaving = false;
  protected error = '';

  protected readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(120)]],
    description: ['', [Validators.required, Validators.maxLength(1000)]],
    dueDate: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.loadTasks();
  }

  protected loadTasks(): void {
    this.isLoading = true;
    this.taskService.getAll()
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (tasks) => (this.tasks = tasks),
        error: () => (this.error = 'Could not load tasks.')
      });
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error = '';
    this.isSaving = true;

    const request = this.buildRequest();
    const operation = this.selectedTask
      ? this.taskService.update(this.selectedTask.id, request)
      : this.taskService.create(request);

    operation.pipe(finalize(() => (this.isSaving = false))).subscribe({
      next: () => {
        this.resetForm();
        this.loadTasks();
      },
      error: () => (this.error = 'Could not save task. Check the required fields and due date.')
    });
  }

  protected edit(task: TaskItem): void {
    this.selectedTask = task;
    this.form.setValue({
      title: task.title,
      description: task.description,
      dueDate: task.dueDate.substring(0, 10)
    });
  }

  protected complete(task: TaskItem): void {
    this.taskService.changeStatus(task.id, { status: TaskStatus.Completed }).subscribe({
      next: () => this.loadTasks(),
      error: () => (this.error = 'Could not change task status.')
    });
  }

  protected delete(task: TaskItem): void {
    const confirmed = window.confirm(`Delete task "${task.title}"?`);
    if (!confirmed) {
      return;
    }

    this.taskService.delete(task.id).subscribe({
      next: () => this.loadTasks(),
      error: () => (this.error = 'Could not delete task.')
    });
  }

  protected resetForm(): void {
    this.selectedTask = null;
    this.form.reset({ title: '', description: '', dueDate: '' });
  }

  protected statusLabel(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Pending:
        return 'Pending';
      case TaskStatus.InProgress:
        return 'In progress';
      case TaskStatus.Completed:
        return 'Completed';
      case TaskStatus.Cancelled:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }

  protected statusClass(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Completed:
        return 'bg-emerald-50 text-emerald-700';
      case TaskStatus.InProgress:
        return 'bg-blue-50 text-blue-700';
      case TaskStatus.Cancelled:
        return 'bg-red-50 text-red-700';
      default:
        return 'bg-slate-100 text-slate-700';
    }
  }

  private buildRequest(): CreateTaskRequest {
    const raw = this.form.getRawValue();
    return {
      title: raw.title.trim(),
      description: raw.description.trim(),
      dueDate: new Date(`${raw.dueDate}T12:00:00.000Z`).toISOString()
    };
  }
}
