namespace TaskManager.Application.Abstractions;

/// <summary>
/// Service for generating AI-powered suggestions for tasks.
/// </summary>
public interface IAiSuggestionService
{
    /// <summary>
    /// Generate a task description suggestion based on the task title.
    /// </summary>
    /// <param name="title">The task title</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Suggested description text</returns>
    Task<string> SuggestDescriptionAsync(string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if AI service is available.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if service is available, false otherwise</returns>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}
