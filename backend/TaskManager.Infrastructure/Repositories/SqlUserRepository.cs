using Microsoft.Data.SqlClient;
using TaskManager.Application.Abstractions;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public sealed class SqlUserRepository : IUserRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SqlUserRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = "SELECT Id, Email, PasswordHash, CreatedAtUtc FROM dbo.Users WHERE Email = @Email;";
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Email", email.Trim().ToLowerInvariant()));

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = "SELECT Id, Email, PasswordHash, CreatedAtUtc FROM dbo.Users WHERE Id = @Id;";
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", id));

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task CreateAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO dbo.Users (Id, Email, PasswordHash, CreatedAtUtc)
            VALUES (@Id, @Email, @PasswordHash, @CreatedAtUtc);";

        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", user.Id));
        command.Parameters.Add(new SqlParameter("@Email", user.Email));
        command.Parameters.Add(new SqlParameter("@PasswordHash", user.PasswordHash));
        command.Parameters.Add(new SqlParameter("@CreatedAtUtc", user.CreatedAtUtc));

        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static User Map(SqlDataReader reader)
    {
        return User.Restore(reader.GetGuid(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3));
    }
}
