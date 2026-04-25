using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.ApiTests.Support;
using TaskManager.Application.Tasks;

namespace TaskManager.ApiTests.Tasks;

public sealed class TaskValidationEndpointTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public TaskValidationEndpointTests(ApiTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_Title_Is_Empty()
    {
        var token = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(token);

        var response = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest(" ", "Description", DateTime.UtcNow.AddDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Should_Return_BadRequest_When_DueDate_Is_In_The_Past()
    {
        var token = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(token);

        var response = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Title", "Description", DateTime.UtcNow.AddDays(-1)));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
