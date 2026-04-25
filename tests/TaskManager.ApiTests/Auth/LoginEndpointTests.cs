using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.Application.Users;
using TaskManager.ApiTests.Support;

namespace TaskManager.ApiTests.Auth;

public sealed class LoginEndpointTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public LoginEndpointTests(ApiTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var email = $"login-{Guid.NewGuid():N}@taskmanager.com";
        var password = "Password@123";
        var register = await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest(email, password));
        register.EnsureSuccessStatusCode();

        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest(email, password));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Password_Is_Invalid()
    {
        var email = $"invalid-password-{Guid.NewGuid():N}@taskmanager.com";
        var register = await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest(email, "Password@123"));
        register.EnsureSuccessStatusCode();

        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest(email, "wrong-password"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_User_Does_Not_Exist()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest($"missing-{Guid.NewGuid():N}@taskmanager.com", "Password@123"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
