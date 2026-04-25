using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.ApiTests.Support;
using TaskManager.Application.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.ApiTests.Tasks;

public sealed class TaskCrudEndpointTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public TaskCrudEndpointTests(ApiTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Post_Should_Create_Task_When_Request_Is_Valid()
    {
        var token = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(token);
        var request = new CreateTaskRequest("Create API test", "Validate task creation", DateTime.UtcNow.AddDays(3));

        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<TaskResponse>();
        body!.Title.Should().Be(request.Title);
        body.Status.Should().Be(TaskStatus.Pending);
    }

    [Fact]
    public async Task Get_Should_Return_User_Tasks()
    {
        var token = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(token);
        var created = await CreateTaskAsync("List API test");

        var response = await _client.GetAsync("/api/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<TaskResponse[]>();
        tasks.Should().Contain(x => x.Id == created.Id);
    }

    [Fact]
    public async Task GetById_Should_Return_Task_When_User_Owns_It()
    {
        var token = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(token);
        var created = await CreateTaskAsync("Get by id API test");

        var response = await _client.GetAsync($"/api/tasks/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        task!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task Put_Should_Update_Task_When_User_Owns_It()
    {
        var token = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(token);
        var created = await CreateTaskAsync("Old title");
        var request = new UpdateTaskRequest("Updated title", "Updated description", DateTime.UtcNow.AddDays(10));

        var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        task!.Title.Should().Be("Updated title");
        task.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task PatchStatus_Should_Change_Task_Status()
    {
        var token = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(token);
        var created = await CreateTaskAsync("Patch status API test");

        var response = await _client.PatchAsJsonAsync($"/api/tasks/{created.Id}/status", new ChangeTaskStatusRequest(TaskStatus.Completed));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        task!.Status.Should().Be(TaskStatus.Completed);
    }

    [Fact]
    public async Task Delete_Should_Remove_Task_When_User_Owns_It()
    {
        var token = await AuthTestClient.RegisterAndGetTokenAsync(_client);
        _client.UseBearerToken(token);
        var created = await CreateTaskAsync("Delete API test");

        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{created.Id}");
        var getResponse = await _client.GetAsync($"/api/tasks/{created.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<TaskResponse> CreateTaskAsync(string title)
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest(title, "Description", DateTime.UtcNow.AddDays(2)));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TaskResponse>())!;
    }
}
