using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions;

public interface ITaskRepository
{
    Task<IReadOnlyCollection<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task CreateAsync(TaskItem task, CancellationToken cancellationToken);
    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
}
