namespace TaskManager.Application.Tasks;

public sealed record CreateTaskRequest(string Title, string Description, DateTime DueDate);
public sealed record UpdateTaskRequest(string Title, string Description, DateTime DueDate);
public sealed record ChangeTaskStatusRequest(Domain.Enums.TaskStatus Status);
public sealed record TaskResponse(Guid Id, string Title, string Description, Domain.Enums.TaskStatus Status, DateTime DueDate, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
