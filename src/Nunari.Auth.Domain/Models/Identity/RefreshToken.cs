using Nunari.Auth.Domain.AggregateRoot;
using Nunari.Auth.Domain.Models.Common;

namespace Nunari.Auth.Domain.Models.Identity;

public class RefreshToken : Auditable, IAuditable
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string DeviceInfo { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public User User { get; private set; } = null!;

    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, string deviceInfo, int lifetimeDays = 30) =>
        new()
        {
            UserId = userId,
            Token = token,
            DeviceInfo = deviceInfo,
            ExpiresAt = DateTime.UtcNow.AddDays(lifetimeDays)
        };

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
