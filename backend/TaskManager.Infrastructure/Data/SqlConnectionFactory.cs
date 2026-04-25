using Microsoft.Data.SqlClient;

namespace TaskManager.Infrastructure.Data;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public SqlConnection CreateConnection() => new(_connectionString);
}
