using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;

namespace TaskManager.UnitTests.Domain;

public sealed class TaskItemTests
{
    [Fact]
    public void Constructor_Should_Create_Task_When_Data_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(2);

        var task = new TaskItem(userId, " Write tests ", " Cover domain rules ", dueDate);

        task.Id.Should().NotBeEmpty();
        task.UserId.Should().Be(userId);
        task.Title.Should().Be("Write tests");
        task.Description.Should().Be("Cover domain rules");
        task.Status.Should().Be(TaskStatus.Pending);
        task.DueDate.Should().Be(dueDate);
        task.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        task.UpdatedAtUtc.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Fail_When_Title_Is_Empty(string title)
    {
        var action = () => new TaskItem(Guid.NewGuid(), title, "Description", DateTime.UtcNow.AddDays(1));

        action.Should().Throw<DomainValidationException>()
            .WithMessage("Task title is required.");
    }

    [Fact]
    public void Constructor_Should_Fail_When_Title_Exceeds_Max_Length()
    {
        var action = () => new TaskItem(Guid.NewGuid(), new string('A', 121), "Description", DateTime.UtcNow.AddDays(1));

        action.Should().Throw<DomainValidationException>()
            .WithMessage("Task title cannot exceed 120 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Fail_When_Description_Is_Empty(string description)
    {
        var action = () => new TaskItem(Guid.NewGuid(), "Title", description, DateTime.UtcNow.AddDays(1));

        action.Should().Throw<DomainValidationException>()
            .WithMessage("Task description is required.");
    }

    [Fact]
    public void Constructor_Should_Fail_When_DueDate_Is_In_The_Past()
    {
        var action = () => new TaskItem(Guid.NewGuid(), "Title", "Description", DateTime.UtcNow.AddDays(-1));

        action.Should().Throw<DomainValidationException>()
            .WithMessage("Due date cannot be in the past.");
    }

    [Fact]
    public void Constructor_Should_Fail_When_UserId_Is_Empty()
    {
        var action = () => new TaskItem(Guid.Empty, "Title", "Description", DateTime.UtcNow.AddDays(1));

        action.Should().Throw<DomainValidationException>()
            .WithMessage("User id is required.");
    }

    [Fact]
    public void Update_Should_Change_Editable_Fields_And_Set_UpdatedAt()
    {
        var task = new TaskItem(Guid.NewGuid(), "Old", "Old description", DateTime.UtcNow.AddDays(1));
        var newDueDate = DateTime.UtcNow.AddDays(5);

        task.Update("New", "New description", newDueDate);

        task.Title.Should().Be("New");
        task.Description.Should().Be("New description");
        task.DueDate.Should().Be(newDueDate);
        task.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void ChangeStatus_Should_Update_Status_And_Set_UpdatedAt()
    {
        var task = new TaskItem(Guid.NewGuid(), "Title", "Description", DateTime.UtcNow.AddDays(1));

        task.ChangeStatus(TaskStatus.Completed);

        task.Status.Should().Be(TaskStatus.Completed);
        task.UpdatedAtUtc.Should().NotBeNull();
    }
}
