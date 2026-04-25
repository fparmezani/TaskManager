import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AiSuggestionService } from '../../core/services/ai-suggestion.service';

@Component({
  selector: 'app-ai-suggestion-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50" (click)="onBackdropClick()">
      <div class="bg-white rounded-lg shadow-lg p-6 w-full max-w-md" (click)="$event.stopPropagation()">
        <h2 class="text-xl font-bold text-slate-900 mb-4">✨ Task Description Suggestion</h2>
        
        <div class="mb-4">
          <label class="block text-sm font-semibold text-slate-700 mb-2">Task Title</label>
          <input 
            type="text" 
            [(ngModel)]="taskTitle" 
            placeholder="Enter task title"
            class="w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-slate-900"
          />
        </div>

        <div *ngIf="suggestion" class="mb-4 p-3 bg-blue-50 rounded-lg border border-blue-200">
          <p class="text-sm text-slate-600"><strong>Suggestion:</strong></p>
          <p class="text-slate-800 mt-2">{{ suggestion }}</p>
        </div>

        <div *ngIf="loading" class="mb-4 p-3 bg-yellow-50 rounded-lg">
          <p class="text-sm text-yellow-700">🔄 Generating suggestion...</p>
        </div>

        <div *ngIf="error" class="mb-4 p-3 bg-red-50 rounded-lg border border-red-200">
          <p class="text-sm text-red-700">{{ error }}</p>
        </div>

        <div class="flex gap-3 justify-end">
          <button
            (click)="onClose.emit()"
            class="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-50"
          >
            Close
          </button>
          <button 
            (click)="generateSuggestion()"
            [disabled]="!taskTitle.trim() || loading"
            class="px-4 py-2 rounded-lg bg-slate-900 text-white hover:bg-slate-700 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ loading ? 'Generating...' : 'Generate' }}
          </button>
          <button 
            *ngIf="suggestion"
            (click)="useSuggestion()"
            class="px-4 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-700"
          >
            Use This
          </button>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class AiSuggestionDialogComponent implements OnInit {
  private readonly aiService = inject(AiSuggestionService);

  @Input() taskTitle = '';
  @Output() onClose = new EventEmitter<void>();
  @Output() onSuggestionSelected = new EventEmitter<string>();

  suggestion = '';
  loading = false;
  error = '';
  isAvailable = false;

  ngOnInit() {
    this.checkAiAvailability();
  }

  checkAiAvailability() {
    this.aiService.checkAvailability().subscribe({
      next: (response) => {
        this.isAvailable = response.available;
        if (!this.isAvailable) {
          this.error = 'AI service is currently unavailable. Please try again later.';
        }
      },
      error: () => {
        this.isAvailable = false;
        this.error = 'Unable to connect to AI service.';
      }
    });
  }

  generateSuggestion() {
    if (!this.taskTitle.trim()) {
      this.error = 'Please enter a task title';
      return;
    }

    this.loading = true;
    this.error = '';
    this.suggestion = '';

    this.aiService.suggestDescription(this.taskTitle).subscribe({
      next: (response) => {
        this.suggestion = response.suggestion;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to generate suggestion. Please try again.';
      }
    });
  }

  useSuggestion() {
    this.onSuggestionSelected.emit(this.suggestion);
    this.onClose.emit();
  }

  onBackdropClick() {
    this.onClose.emit();
  }
}
