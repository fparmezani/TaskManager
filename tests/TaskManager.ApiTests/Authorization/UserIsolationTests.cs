using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.ApiTests.Support;
using TaskManager.Application.Tasks;

namespace TaskManager.ApiTests.Authorization;

public sealed class UserIsolationTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public UserIsolationTests(ApiTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task User_A_Should_Not_Get_Task_From_User_B()
    {
        var userAToken = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(userAToken);
        var userATask = await CreateTaskAsync("Private User A task");

        var userBToken = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(userBToken);

        var response = await _client.GetAsync($"/api/tasks/{userATask.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task User_A_Should_Not_Update_Task_From_User_B()
    {
        var userAToken = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(userAToken);
        var userATask = await CreateTaskAsync("Private User A task");

        var userBToken = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(userBToken);

        var response = await _client.PutAsJsonAsync($"/api/tasks/{userATask.Id}", new UpdateTaskRequest("Hacked", "Should not work", DateTime.UtcNow.AddDays(3)));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task User_A_Should_Not_Delete_Task_From_User_B()
    {
        var userAToken = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(userAToken);
        var userATask = await CreateTaskAsync("Private User A task");

        var userBToken = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(userBToken);
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{userATask.Id}");

        _client.UseBearerToken(userAToken);
        var getResponse = await _client.GetAsync($"/api/tasks/{userATask.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<TaskResponse> CreateTaskAsync(string title)
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest(title, "Description", DateTime.UtcNow.AddDays(2)));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TaskResponse>())!;
    }
}
