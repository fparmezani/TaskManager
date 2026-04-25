using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.ApiTests.Support;
using TaskManager.Application.Users;

namespace TaskManager.ApiTests.Auth;

public sealed class RegisterEndpointTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public RegisterEndpointTests(ApiTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Register_Should_Return_Created_When_Request_Is_Valid()
    {
        var request = new RegisterUserRequest($"register-{Guid.NewGuid():N}@taskmanager.com", "Password@123");

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body!.Email.Should().Be(request.Email);
        body.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_Should_Return_BadRequest_When_Email_Already_Exists()
    {
        var request = new RegisterUserRequest($"duplicate-{Guid.NewGuid():N}@taskmanager.com", "Password@123");
        var first = await _client.PostAsJsonAsync("/api/auth/register", request);
        first.EnsureSuccessStatusCode();

        var second = await _client.PostAsJsonAsync("/api/auth/register", request);

        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_Should_Return_BadRequest_When_Password_Is_Too_Short()
    {
        var request = new RegisterUserRequest($"short-{Guid.NewGuid():N}@taskmanager.com", "123");

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
