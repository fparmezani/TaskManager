using System.Text;
using System.Text.Json;
using TaskManager.Application.Abstractions;

namespace TaskManager.Infrastructure.AI;

/// <summary>
/// AI suggestion service using Ollama for local LLM inference.
/// </summary>
public sealed class OllamaAiSuggestionService : IAiSuggestionService
{
    private readonly HttpClient _httpClient;
    private readonly string _ollamaBaseUrl;
    private readonly string _model;

    public OllamaAiSuggestionService(HttpClient httpClient, string ollamaBaseUrl = "http://ollama:11434", string model = "mistral")
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ollamaBaseUrl = ollamaBaseUrl ?? throw new ArgumentNullException(nameof(ollamaBaseUrl));
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public async Task<string> SuggestDescriptionAsync(string title, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        var prompt = $"""
            Generate a brief and professional task description (max 200 characters) for a task with the title: "{title}"
            
            Return only the description text, without any additional commentary.
            """;

        try
        {
            var requestBody = new
            {
                model = _model,
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"{_ollamaBaseUrl}/api/generate",
                content,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Ollama request failed with status {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

            if (jsonResponse.TryGetProperty("response", out var responseProperty))
            {
                var suggestion = responseProperty.GetString()?.Trim() ?? string.Empty;
                // Remove common prefixes that the model might add
                suggestion = RemoveCommonPrefixes(suggestion);
                return suggestion.Length > 200 ? suggestion[..200] : suggestion;
            }

            throw new InvalidOperationException("Invalid response format from Ollama");
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Failed to connect to Ollama service. Ensure Ollama is running.", ex);
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{_ollamaBaseUrl}/api/tags",
                cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static string RemoveCommonPrefixes(string text)
    {
        var prefixes = new[] { "Here's", "Here is", "The description:", "Description:", "Task description:" };

        foreach (var prefix in prefixes)
        {
            if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                var result = text[prefix.Length..].TrimStart();
                // Remove leading quotes if present
                if (result.StartsWith("\"") && result.EndsWith("\""))
                    result = result[1..^1];
                return result.TrimStart();
            }
        }

        return text;
    }
}
