using Nunari.Auth.Domain.Enums;
using Nunari.Auth.Domain.Events;
using Nunari.Auth.Domain.Models.Identity;

namespace Nunari.Auth.Domain.AggregateRoot;

public class User : AggregateRoot
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string FullName
    {
        get { return $"{FirstName} {LastName}"; }
    }

    public UserState State { get; set; }

    public string? PictureUrl { get; set; }

    // OAuth
    public string? GoogleId { get; private set; }
    public string? AppleId { get; private set; }

    // Identity navigation properties
    public virtual ICollection<UserClaim>? Claims { get; set; }
    public virtual ICollection<UserLogin>? Logins { get; set; }
    public virtual ICollection<UserToken>? Tokens { get; set; }
    public virtual ICollection<UserRole>? UserRoles { get; set; }
    public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
    public virtual ICollection<PasswordResetToken>? PasswordResetTokens { get; set; }

    protected User() { }

    public static User CreateWithPassword(string email)
    {
        var user = new User
        {
            Email = email,
            UserName = email,
            State = UserState.Active,
            CreatedOn = DateTime.UtcNow,
        };

        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email));

        return user;
    }

    public static User CreateWithOAuth(string email, string? pictureUrl,
        string? googleId = null, string? appleId = null)
    {
        var user = new User
        {
            Email = email,
            UserName = email,
            PictureUrl = pictureUrl,
            GoogleId = googleId,
            AppleId = appleId,
            EmailConfirmed = true, // OAuth emails are pre-verified
            State = UserState.Active,
            CreatedOn = DateTime.UtcNow,
        };

        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email));

        return user;
    }

    public void Deactivate()
    {
        State = UserState.Inactive;
        AddDomainEvent(new UserDeactivatedDomainEvent(Id));
    }

    public void Activate()
    {
        State = UserState.Active;
        AddDomainEvent(new UserActivatedDomainEvent(Id));
    }

    public void Block()
    {
        State = UserState.Blocked;
        AddDomainEvent(new UserActivatedDomainEvent(Id));
    }

    public void ChangePassword(string newHash)
    {
        PasswordHash = newHash;
        AddDomainEvent(new PasswordChangedDomainEvent(Id));
    }

    public void AddRole(UserRole role)
    {
        if (!UserRoles!.Contains(role))
        {
            UserRoles.Add(role);
            AddDomainEvent(new RoleAddedToUserDomainEvent(Id, role.Role?.Name!));
        }
    }

    public void RemoveRole(UserRole role)
    {
        if (UserRoles!.Remove(role))
        {
            AddDomainEvent(new RoleRemovedFromUserDomainEvent(Id, role.Role?.Name!));
        }
    }
    public void LinkGoogle(string googleId) { GoogleId = googleId; }
    public void LinkApple(string appleId) { AppleId = appleId; }
    public void VerifyEmail() { EmailConfirmed = true; }
    //public void UpdateProfile(string displayName, string? avatarUrl)
    //{
    //    DisplayName = displayName;
    //    AvatarUrl = avatarUrl;
    //}

    public void SetPasswordHash(string hash) { PasswordHash = hash; }
}
