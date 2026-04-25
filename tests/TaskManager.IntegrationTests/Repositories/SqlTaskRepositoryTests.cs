using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Repositories;
using TaskManager.IntegrationTests.Support;

namespace TaskManager.IntegrationTests.Repositories;

public sealed class SqlTaskRepositoryTests : IClassFixture<SqlServerTestFixture>
{
    private readonly SqlUserRepository _userRepository;
    private readonly SqlTaskRepository _taskRepository;

    public SqlTaskRepositoryTests(SqlServerTestFixture fixture)
    {
        _userRepository = new SqlUserRepository(fixture.ConnectionFactory);
        _taskRepository = new SqlTaskRepository(fixture.ConnectionFactory);
    }

    [Fact]
    public async Task CreateAsync_Should_Insert_Task()
    {
        var user = await CreateUserAsync();
        var task = new TaskItem(user.Id, "Insert task", "Description", DateTime.UtcNow.AddDays(1));

        await _taskRepository.CreateAsync(task, CancellationToken.None);

        var saved = await _taskRepository.GetByIdAsync(task.Id, user.Id, CancellationToken.None);
        saved.Should().NotBeNull();
        saved!.Title.Should().Be(task.Title);
    }

    [Fact]
    public async Task GetByUserIdAsync_Should_Return_Only_User_Tasks()
    {
        var userA = await CreateUserAsync();
        var userB = await CreateUserAsync();
        var taskA = new TaskItem(userA.Id, "User A task", "Description", DateTime.UtcNow.AddDays(1));
        var taskB = new TaskItem(userB.Id, "User B task", "Description", DateTime.UtcNow.AddDays(1));
        await _taskRepository.CreateAsync(taskA, CancellationToken.None);
        await _taskRepository.CreateAsync(taskB, CancellationToken.None);

        var result = await _taskRepository.GetByUserIdAsync(userA.Id, CancellationToken.None);

        result.Should().ContainSingle(x => x.Id == taskA.Id);
        result.Should().NotContain(x => x.Id == taskB.Id);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Task()
    {
        var user = await CreateUserAsync();
        var task = new TaskItem(user.Id, "Old", "Old description", DateTime.UtcNow.AddDays(1));
        await _taskRepository.CreateAsync(task, CancellationToken.None);

        task.Update("New", "New description", DateTime.UtcNow.AddDays(5));
        task.ChangeStatus(TaskStatus.Completed);
        await _taskRepository.UpdateAsync(task, CancellationToken.None);

        var saved = await _taskRepository.GetByIdAsync(task.Id, user.Id, CancellationToken.None);
        saved!.Title.Should().Be("New");
        saved.Status.Should().Be(TaskStatus.Completed);
        saved.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Task()
    {
        var user = await CreateUserAsync();
        var task = new TaskItem(user.Id, "Delete", "Description", DateTime.UtcNow.AddDays(1));
        await _taskRepository.CreateAsync(task, CancellationToken.None);

        await _taskRepository.DeleteAsync(task.Id, user.Id, CancellationToken.None);

        var saved = await _taskRepository.GetByIdAsync(task.Id, user.Id, CancellationToken.None);
        saved.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Not_Return_Task_From_Another_User()
    {
        var owner = await CreateUserAsync();
        var otherUser = await CreateUserAsync();
        var task = new TaskItem(owner.Id, "Private", "Description", DateTime.UtcNow.AddDays(1));
        await _taskRepository.CreateAsync(task, CancellationToken.None);

        var result = await _taskRepository.GetByIdAsync(task.Id, otherUser.Id, CancellationToken.None);

        result.Should().BeNull();
    }

    private async Task<User> CreateUserAsync()
    {
        var user = new User($"user-{Guid.NewGuid():N}@taskmanager.com", "hash");
        await _userRepository.CreateAsync(user, CancellationToken.None);
        return user;
    }
}
