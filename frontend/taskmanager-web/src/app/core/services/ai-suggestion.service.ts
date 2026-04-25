import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AiSuggestionResponse {
  suggestion: string;
}

export interface AiAvailabilityResponse {
  available: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AiSuggestionService {
  private readonly http = inject(HttpClient);

  suggestDescription(title: string): Observable<AiSuggestionResponse> {
    return this.http.post<AiSuggestionResponse>(
      '/api/tasks/suggestions/description',
      null,
      { params: { title } }
    );
  }

  checkAvailability(): Observable<AiAvailabilityResponse> {
    return this.http.get<AiAvailabilityResponse>('/api/tasks/suggestions/available');
  }
}
