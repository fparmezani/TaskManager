using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace TaskManager.ApiTests.Support;

public sealed class ApiTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithPassword("Your_strong_password123!")
        .Build();

    public TaskManagerWebApplicationFactory Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        var builder = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "TaskManagerDb"
        };
        var connectionString = builder.ConnectionString;
        Factory = new TaskManagerWebApplicationFactory(connectionString);
        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
        await _container.DisposeAsync();
    }
}
