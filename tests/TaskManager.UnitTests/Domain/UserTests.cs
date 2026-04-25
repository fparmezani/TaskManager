using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.UnitTests.Domain;

public sealed class UserTests
{
    [Fact]
    public void Constructor_Should_Create_User_When_Data_Is_Valid()
    {
        var user = new User(" Demo@TaskManager.com ", "hash-value");

        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be("demo@taskmanager.com");
        user.PasswordHash.Should().Be("hash-value");
        user.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Fail_When_Email_Is_Empty(string email)
    {
        var action = () => new User(email, "hash-value");

        action.Should().Throw<DomainValidationException>()
            .WithMessage("Email is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Fail_When_PasswordHash_Is_Empty(string passwordHash)
    {
        var action = () => new User("demo@taskmanager.com", passwordHash);

        action.Should().Throw<DomainValidationException>()
            .WithMessage("Password hash is required.");
    }
}
