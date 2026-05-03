using Nunari.Auth.Domain.AggregateRoot;
using Nunari.Auth.Domain.Models.Common;

namespace Nunari.Auth.Domain.Models.Identity;

public class PasswordResetToken: Auditable, IAuditable
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty; // store hashed, send raw
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsUsed { get; private set; }

    public User User { get; private set; } = null!;

    public bool IsValid => !IsUsed && DateTime.UtcNow < ExpiresAt;

    private PasswordResetToken() { }

    public static PasswordResetToken Create(Guid userId, string tokenHash, int lifetimeMinutes = 15) =>
        new()
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(lifetimeMinutes)
        };

    public void MarkUsed() => IsUsed = true;
}

