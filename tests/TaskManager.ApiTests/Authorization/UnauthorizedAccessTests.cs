using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.ApiTests.Support;
using TaskManager.Application.Tasks;

namespace TaskManager.ApiTests.Authorization;

public sealed class UnauthorizedAccessTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public UnauthorizedAccessTests(ApiTestFixture fixture)
    {
        _client = fixture.Client;
        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Get_Tasks_Should_Return_Unauthorized_When_No_Token()
    {
        var response = await _client.GetAsync("/api/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Tasks_Should_Return_Unauthorized_When_No_Token()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Title", "Description", DateTime.UtcNow.AddDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Put_Tasks_Should_Return_Unauthorized_When_No_Token()
    {
        var response = await _client.PutAsJsonAsync($"/api/tasks/{Guid.NewGuid()}", new UpdateTaskRequest("Title", "Description", DateTime.UtcNow.AddDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_Tasks_Should_Return_Unauthorized_When_No_Token()
    {
        var response = await _client.DeleteAsync($"/api/tasks/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Secure_Endpoint_Should_Return_Unauthorized_When_No_Token()
    {
        var response = await _client.GetAsync("/api/auth/secure");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Public_Endpoint_Should_Return_Ok_When_No_Token()
    {
        var response = await _client.GetAsync("/api/auth/public");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
