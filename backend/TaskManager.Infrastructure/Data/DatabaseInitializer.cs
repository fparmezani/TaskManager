using Microsoft.Data.SqlClient;

namespace TaskManager.Infrastructure.Data;

public sealed class DatabaseInitializer
{
    private const string DatabaseName = "TaskManagerDb";
    private readonly string _connectionString;

    public DatabaseInitializer(ISqlConnectionFactory connectionFactory)
    {
        // The initializer intentionally reads the final application connection string.
        // It first connects to master to create the database if it does not exist,
        // then reconnects to TaskManagerDb to create tables and seed demo data.
        _connectionString = connectionFactory.CreateConnection().ConnectionString;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDatabaseExistsAsync(cancellationToken);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        foreach (var sql in SchemaAndSeedScripts)
        {
            await using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private async Task EnsureDatabaseExistsAsync(CancellationToken cancellationToken)
    {
        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "master"
        };

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = $@"
IF DB_ID('{DatabaseName}') IS NULL
BEGIN
    CREATE DATABASE [{DatabaseName}];
END";

        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static readonly string[] SchemaAndSeedScripts =
    [
        @"IF OBJECT_ID('dbo.Users', 'U') IS NULL
          BEGIN
              CREATE TABLE dbo.Users (
                  Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                  Email NVARCHAR(256) NOT NULL UNIQUE,
                  PasswordHash NVARCHAR(500) NOT NULL,
                  CreatedAtUtc DATETIME2 NOT NULL
              );
          END",
        @"IF OBJECT_ID('dbo.Tasks', 'U') IS NULL
          BEGIN
              CREATE TABLE dbo.Tasks (
                  Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                  UserId UNIQUEIDENTIFIER NOT NULL,
                  Title NVARCHAR(120) NOT NULL,
                  Description NVARCHAR(1000) NOT NULL,
                  Status INT NOT NULL,
                  DueDate DATETIME2 NOT NULL,
                  CreatedAtUtc DATETIME2 NOT NULL,
                  UpdatedAtUtc DATETIME2 NULL,
                  CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
              );
          END",
        @"IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'demo@taskmanager.com')
          BEGIN
              DECLARE @DemoUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
              INSERT INTO dbo.Users (Id, Email, PasswordHash, CreatedAtUtc)
              VALUES (@DemoUserId, 'demo@taskmanager.com', 'PBKDF2-SHA256$10000$dGFza21hbmFnZXItZGVtby1zYWx0LTIwMjY=$kD0KOici0orLRHViNe1AGvpH11+JgHPW3x7suuvZLbg=', SYSUTCDATETIME());

              INSERT INTO dbo.Tasks (Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc)
              VALUES
              (NEWID(), @DemoUserId, 'Finish technical interview project', 'Complete API, frontend, tests, Docker setup and README.', 2, DATEADD(day, 7, SYSUTCDATETIME()), SYSUTCDATETIME(), NULL),
              (NEWID(), @DemoUserId, 'Review Clean Architecture presentation', 'Prepare a concise explanation of layers and dependencies.', 1, DATEADD(day, 10, SYSUTCDATETIME()), SYSUTCDATETIME(), NULL),
              (NEWID(), @DemoUserId, 'Prepare GenAI prompt engineering explanation', 'Document validation, corrections and edge cases handled after AI output.', 1, DATEADD(day, 14, SYSUTCDATETIME()), SYSUTCDATETIME(), NULL);
          END"
    ];
}
