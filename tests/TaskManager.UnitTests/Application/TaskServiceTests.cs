using FluentAssertions;
using NSubstitute;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;

namespace TaskManager.UnitTests.Application;

public sealed class TaskServiceTests
{
    private readonly ITaskRepository _repository = Substitute.For<ITaskRepository>();
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _service = new TaskService(_repository);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Task_When_Request_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var request = new CreateTaskRequest("Title", "Description", DateTime.UtcNow.AddDays(1));

        var response = await _service.CreateAsync(userId, request, CancellationToken.None);

        response.Title.Should().Be(request.Title);
        response.Description.Should().Be(request.Description);
        response.Status.Should().Be(TaskStatus.Pending);
        await _repository.Received(1).CreateAsync(Arg.Is<TaskItem>(x => x.UserId == userId && x.Title == request.Title), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_When_Title_Is_Empty()
    {
        var request = new CreateTaskRequest(" ", "Description", DateTime.UtcNow.AddDays(1));

        var action = () => _service.CreateAsync(Guid.NewGuid(), request, CancellationToken.None);

        await action.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("Task title is required.");
        await _repository.DidNotReceive().CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Tasks_For_User()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem(userId, "Task", "Description", DateTime.UtcNow.AddDays(1));
        _repository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(new[] { task });

        var result = await _service.GetAllAsync(userId, CancellationToken.None);

        result.Should().ContainSingle(x => x.Id == task.Id && x.Title == task.Title);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Task_Does_Not_Exist()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((TaskItem?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Task not found.");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Task_When_User_Owns_Task()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem(userId, "Old", "Old description", DateTime.UtcNow.AddDays(1));
        _repository.GetByIdAsync(task.Id, userId, Arg.Any<CancellationToken>()).Returns(task);
        var request = new UpdateTaskRequest("New", "New description", DateTime.UtcNow.AddDays(5));

        var result = await _service.UpdateAsync(task.Id, userId, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("New");
        await _repository.Received(1).UpdateAsync(Arg.Is<TaskItem>(x => x.Id == task.Id && x.Title == "New"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_Task_Does_Not_Exist()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((TaskItem?)null);

        var result = await _service.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), new UpdateTaskRequest("Title", "Description", DateTime.UtcNow.AddDays(1)), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ChangeStatusAsync_Should_Update_Status_When_Task_Exists()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem(userId, "Title", "Description", DateTime.UtcNow.AddDays(1));
        _repository.GetByIdAsync(task.Id, userId, Arg.Any<CancellationToken>()).Returns(task);

        var result = await _service.ChangeStatusAsync(task.Id, userId, new ChangeTaskStatusRequest(TaskStatus.Completed), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(TaskStatus.Completed);
        await _repository.Received(1).UpdateAsync(Arg.Is<TaskItem>(x => x.Status == TaskStatus.Completed), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_Should_Call_Repository_With_User_Scope()
    {
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _service.DeleteAsync(taskId, userId, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(taskId, userId, Arg.Any<CancellationToken>());
    }
}
