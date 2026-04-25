using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Users;

public sealed class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        if (request.Password.Length < 8)
            return Result<AuthResponse>.Failure("Password must contain at least 8 characters.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
            return Result<AuthResponse>.Failure("Email already registered.");

        var user = new User(normalizedEmail, _passwordHasher.Hash(request.Password));
        await _userRepository.CreateAsync(user, cancellationToken);

        var token = _jwtTokenGenerator.Generate(user);
        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Email, token));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Invalid credentials.");

        var token = _jwtTokenGenerator.Generate(user);
        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Email, token));
    }
}
