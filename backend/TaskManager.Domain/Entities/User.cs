using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainValidationException("Password hash is required.");

        Id = Guid.NewGuid();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static User Restore(Guid id, string email, string passwordHash, DateTime createdAtUtc)
    {
        return new User
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAtUtc = createdAtUtc
        };
    }
}
