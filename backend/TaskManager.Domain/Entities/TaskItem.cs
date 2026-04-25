using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

public sealed class TaskItem
{
    private const int MaxTitleLength = 120;
    private const int MaxDescriptionLength = 1000;

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Enums.TaskStatus Status { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private TaskItem()
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    public TaskItem(Guid userId, string title, string description, DateTime dueDate)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("User id is required.");

        Validate(title, description, dueDate);

        Id = Guid.NewGuid();
        UserId = userId;
        Title = title.Trim();
        Description = description.Trim();
        DueDate = dueDate;
        Status = Enums.TaskStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static TaskItem Restore(Guid id, Guid userId, string title, string description, Enums.TaskStatus status, DateTime dueDate, DateTime createdAtUtc, DateTime? updatedAtUtc)
    {
        return new TaskItem
        {
            Id = id,
            UserId = userId,
            Title = title,
            Description = description,
            Status = status,
            DueDate = dueDate,
            CreatedAtUtc = createdAtUtc,
            UpdatedAtUtc = updatedAtUtc
        };
    }

    public void Update(string title, string description, DateTime dueDate)
    {
        Validate(title, description, dueDate);
        Title = title.Trim();
        Description = description.Trim();
        DueDate = dueDate;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(Enums.TaskStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static void Validate(string title, string description, DateTime dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainValidationException("Task title is required.");
        if (title.Length > MaxTitleLength)
            throw new DomainValidationException($"Task title cannot exceed {MaxTitleLength} characters.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainValidationException("Task description is required.");
        if (description.Length > MaxDescriptionLength)
            throw new DomainValidationException($"Task description cannot exceed {MaxDescriptionLength} characters.");
        if (dueDate.Date < DateTime.UtcNow.Date)
            throw new DomainValidationException("Due date cannot be in the past.");
    }
}
