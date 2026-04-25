using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tasks;

public sealed class TaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(userId, cancellationToken);
        return tasks.Select(Map).ToArray();
    }

    public async Task<Result<TaskResponse>> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(id, userId, cancellationToken);
        return task is null ? Result<TaskResponse>.Failure("Task not found.") : Result<TaskResponse>.Success(Map(task));
    }

    public async Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = new TaskItem(userId, request.Title, request.Description, request.DueDate);
        await _taskRepository.CreateAsync(task, cancellationToken);
        return Map(task);
    }

    public async Task<Result<TaskResponse>> UpdateAsync(Guid id, Guid userId, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(id, userId, cancellationToken);
        if (task is null)
            return Result<TaskResponse>.Failure("Task not found.");

        task.Update(request.Title, request.Description, request.DueDate);
        await _taskRepository.UpdateAsync(task, cancellationToken);
        return Result<TaskResponse>.Success(Map(task));
    }

    public async Task<Result<TaskResponse>> ChangeStatusAsync(Guid id, Guid userId, ChangeTaskStatusRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(id, userId, cancellationToken);
        if (task is null)
            return Result<TaskResponse>.Failure("Task not found.");

        task.ChangeStatus(request.Status);
        await _taskRepository.UpdateAsync(task, cancellationToken);
        return Result<TaskResponse>.Success(Map(task));
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        await _taskRepository.DeleteAsync(id, userId, cancellationToken);
    }

    private static TaskResponse Map(TaskItem task)
    {
        return new TaskResponse(task.Id, task.Title, task.Description, task.Status, task.DueDate, task.CreatedAtUtc, task.UpdatedAtUtc);
    }
}
