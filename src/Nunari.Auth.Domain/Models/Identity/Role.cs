using Microsoft.AspNetCore.Identity;
using Nunari.Auth.Domain.AggregateRoot;
using Nunari.Auth.Domain.Models.Common;

namespace Nunari.Auth.Domain.Models.Identity;

public class Role : IdentityRole<Guid>, IAuditable
{
    public virtual ICollection<UserRole>? UserRoles { get; set; }
    public virtual ICollection<RoleClaim>? RoleClaims { get; set; }

    // Auditable

    public bool IsDeleted { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public Guid? UpdatedById { get; set; }
    public User? UpdatedBy { get; set; }

    public Role() { }

    public Role(string roleName) => Name = roleName;
}
