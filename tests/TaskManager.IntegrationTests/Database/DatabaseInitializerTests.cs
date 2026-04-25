using FluentAssertions;
using Microsoft.Data.SqlClient;
using TaskManager.Infrastructure.Data;
using TaskManager.IntegrationTests.Support;

namespace TaskManager.IntegrationTests.Database;

public sealed class DatabaseInitializerTests : IClassFixture<SqlServerTestFixture>
{
    private readonly SqlServerTestFixture _fixture;

    public DatabaseInitializerTests(SqlServerTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task InitializeAsync_Should_Create_Users_Table_When_It_Does_Not_Exist()
    {
        var exists = await TableExistsAsync("Users");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task InitializeAsync_Should_Create_Tasks_Table_When_It_Does_Not_Exist()
    {
        var exists = await TableExistsAsync("Tasks");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task InitializeAsync_Should_Seed_Demo_User_When_Not_Exists()
    {
        await using var connection = _fixture.ConnectionFactory.CreateConnection();
        await using var command = new SqlCommand("SELECT COUNT(1) FROM dbo.Users WHERE Email = 'demo@taskmanager.com';", connection);

        await connection.OpenAsync();
        var count = (int)await command.ExecuteScalarAsync();

        count.Should().Be(1);
    }

    [Fact]
    public async Task InitializeAsync_Should_Not_Duplicate_Seed_When_Run_Twice()
    {
        var initializer = new DatabaseInitializer(_fixture.ConnectionFactory);

        await initializer.InitializeAsync();
        await initializer.InitializeAsync();

        await using var connection = _fixture.ConnectionFactory.CreateConnection();
        await using var usersCommand = new SqlCommand("SELECT COUNT(1) FROM dbo.Users WHERE Email = 'demo@taskmanager.com';", connection);
        await using var tasksCommand = new SqlCommand("SELECT COUNT(1) FROM dbo.Tasks WHERE UserId = '11111111-1111-1111-1111-111111111111';", connection);

        await connection.OpenAsync();
        var usersCount = (int)await usersCommand.ExecuteScalarAsync();
        var tasksCount = (int)await tasksCommand.ExecuteScalarAsync();

        usersCount.Should().Be(1);
        tasksCount.Should().Be(3);
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        await using var connection = _fixture.ConnectionFactory.CreateConnection();
        await using var command = new SqlCommand("SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName;", connection);
        command.Parameters.Add(new SqlParameter("@TableName", tableName));

        await connection.OpenAsync();
        var count = (int)await command.ExecuteScalarAsync();
        return count == 1;
    }
}
