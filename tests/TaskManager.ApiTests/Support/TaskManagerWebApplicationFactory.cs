using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace TaskManager.ApiTests.Support;

public sealed class TaskManagerWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public TaskManagerWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["Jwt:Issuer"] = "TaskManager.ApiTests",
                ["Jwt:Audience"] = "TaskManager.ApiTests",
                ["Jwt:Secret"] = "api-tests-secret-key-with-at-least-32-characters"
            });
        });
    }
}
