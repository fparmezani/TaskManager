using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManager.Application.Users;

namespace TaskManager.ApiTests.Support;

public static class AuthTestClient
{
    public static async Task<string> RegisterAndGetTokenAsync(HttpClient client)
    {
        var email = $"api-{Guid.NewGuid():N}@taskmanager.com";
        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest(email, "Password@123"));
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.AccessToken;
    }

    public static void UseBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static void ClearBearerToken(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }
}
