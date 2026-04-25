using Microsoft.Data.SqlClient;
using TaskManager.Infrastructure.Data;
using Testcontainers.MsSql;

namespace TaskManager.IntegrationTests.Support;

public sealed class SqlServerTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithPassword("Your_strong_password123!")
        .Build();

    public string ConnectionString
    {
        get
        {
            var builder = new SqlConnectionStringBuilder(_container.GetConnectionString())
            {
                InitialCatalog = "TaskManagerDb"
            };
            return builder.ConnectionString;
        }
    }
    public SqlConnectionFactory ConnectionFactory => new(ConnectionString);

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        var initializer = new DatabaseInitializer(ConnectionFactory);
        await initializer.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }
}
