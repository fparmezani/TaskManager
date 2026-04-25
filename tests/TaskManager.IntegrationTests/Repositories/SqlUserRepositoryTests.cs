using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Repositories;
using TaskManager.IntegrationTests.Support;

namespace TaskManager.IntegrationTests.Repositories;

public sealed class SqlUserRepositoryTests : IClassFixture<SqlServerTestFixture>
{
    private readonly SqlUserRepository _repository;

    public SqlUserRepositoryTests(SqlServerTestFixture fixture)
    {
        _repository = new SqlUserRepository(fixture.ConnectionFactory);
    }

    [Fact]
    public async Task CreateAsync_Should_Insert_User()
    {
        var user = new User($"insert-{Guid.NewGuid():N}@taskmanager.com", "hash");

        await _repository.CreateAsync(user, CancellationToken.None);

        var saved = await _repository.GetByEmailAsync(user.Email, CancellationToken.None);
        saved.Should().NotBeNull();
        saved!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_User_When_Email_Exists()
    {
        var user = new User($"get-{Guid.NewGuid():N}@taskmanager.com", "hash");
        await _repository.CreateAsync(user, CancellationToken.None);

        var result = await _repository.GetByEmailAsync(user.Email.ToUpperInvariant(), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        var result = await _repository.GetByEmailAsync($"missing-{Guid.NewGuid():N}@taskmanager.com", CancellationToken.None);

        result.Should().BeNull();
    }
}
