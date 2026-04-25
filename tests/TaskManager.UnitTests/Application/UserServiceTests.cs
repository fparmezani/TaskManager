using FluentAssertions;
using NSubstitute;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Users;
using TaskManager.Domain.Entities;

namespace TaskManager.UnitTests.Application;

public sealed class UserServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_userRepository, _passwordHasher, _jwtTokenGenerator);
    }

    [Fact]
    public async Task RegisterAsync_Should_Register_User_When_Email_Is_New()
    {
        var request = new RegisterUserRequest("new@taskmanager.com", "Password@123");
        _userRepository.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns((User?)null);
        _passwordHasher.Hash(request.Password).Returns("hash");
        _jwtTokenGenerator.Generate(Arg.Any<User>()).Returns("token");

        var result = await _service.RegisterAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Email.Should().Be(request.Email);
        result.Value.AccessToken.Should().Be("token");
        await _userRepository.Received(1).CreateAsync(Arg.Is<User>(x => x.Email == request.Email), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterAsync_Should_Fail_When_Email_Already_Exists()
    {
        var existingUser = new User("demo@taskmanager.com", "hash");
        var request = new RegisterUserRequest(existingUser.Email, "Password@123");
        _userRepository.GetByEmailAsync(existingUser.Email, Arg.Any<CancellationToken>()).Returns(existingUser);

        var result = await _service.RegisterAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Email already registered.");
        await _userRepository.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterAsync_Should_Fail_When_Password_Is_Too_Short()
    {
        var result = await _service.RegisterAsync(new RegisterUserRequest("demo@taskmanager.com", "123"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Password must contain at least 8 characters.");
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var user = new User("demo@taskmanager.com", "hash");
        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("Password@123", user.PasswordHash).Returns(true);
        _jwtTokenGenerator.Generate(user).Returns("token");

        var result = await _service.LoginAsync(new LoginUserRequest(user.Email, "Password@123"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("token");
    }

    [Fact]
    public async Task LoginAsync_Should_Fail_When_User_Does_Not_Exist()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _service.LoginAsync(new LoginUserRequest("missing@taskmanager.com", "Password@123"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid credentials.");
    }

    [Fact]
    public async Task LoginAsync_Should_Fail_When_Password_Is_Invalid()
    {
        var user = new User("demo@taskmanager.com", "hash");
        _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("wrong-password", user.PasswordHash).Returns(false);

        var result = await _service.LoginAsync(new LoginUserRequest(user.Email, "wrong-password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid credentials.");
    }
}
