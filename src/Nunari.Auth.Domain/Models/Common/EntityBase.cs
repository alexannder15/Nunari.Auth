namespace Nunari.Auth.Domain.Models.Common;

public class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsDeleted { get; set; }
}
