using Microsoft.Data.SqlClient;
using TaskManager.Application.Abstractions;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public sealed class SqlTaskRepository : ITaskRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SqlTaskRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc
            FROM dbo.Tasks
            WHERE UserId = @UserId
            ORDER BY DueDate ASC;";

        var tasks = new List<TaskItem>();
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@UserId", userId));

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
            tasks.Add(Map(reader));

        return tasks;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc
            FROM dbo.Tasks
            WHERE Id = @Id AND UserId = @UserId;";

        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", id));
        command.Parameters.Add(new SqlParameter("@UserId", userId));

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task CreateAsync(TaskItem task, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO dbo.Tasks (Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc)
            VALUES (@Id, @UserId, @Title, @Description, @Status, @DueDate, @CreatedAtUtc, @UpdatedAtUtc);";

        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        AddParameters(command, task);

        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken)
    {
        const string sql = @"
            UPDATE dbo.Tasks
            SET Title = @Title,
                Description = @Description,
                Status = @Status,
                DueDate = @DueDate,
                UpdatedAtUtc = @UpdatedAtUtc
            WHERE Id = @Id AND UserId = @UserId;";

        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        AddParameters(command, task);

        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM dbo.Tasks WHERE Id = @Id AND UserId = @UserId;";

        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", id));
        command.Parameters.Add(new SqlParameter("@UserId", userId));

        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddParameters(SqlCommand command, TaskItem task)
    {
        command.Parameters.Add(new SqlParameter("@Id", task.Id));
        command.Parameters.Add(new SqlParameter("@UserId", task.UserId));
        command.Parameters.Add(new SqlParameter("@Title", task.Title));
        command.Parameters.Add(new SqlParameter("@Description", task.Description));
        command.Parameters.Add(new SqlParameter("@Status", (int)task.Status));
        command.Parameters.Add(new SqlParameter("@DueDate", task.DueDate));
        command.Parameters.Add(new SqlParameter("@CreatedAtUtc", task.CreatedAtUtc));
        command.Parameters.Add(new SqlParameter("@UpdatedAtUtc", (object?)task.UpdatedAtUtc ?? DBNull.Value));
    }

    private static TaskItem Map(SqlDataReader reader)
    {
        return TaskItem.Restore(
            reader.GetGuid(0),
            reader.GetGuid(1),
            reader.GetString(2),
            reader.GetString(3),
            (Domain.Enums.TaskStatus)reader.GetInt32(4),
            reader.GetDateTime(5),
            reader.GetDateTime(6),
            reader.IsDBNull(7) ? null : reader.GetDateTime(7));
    }
}
