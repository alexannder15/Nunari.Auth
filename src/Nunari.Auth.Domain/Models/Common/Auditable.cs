using Nunari.Auth.Domain.AggregateRoot;

namespace Nunari.Auth.Domain.Models.Common;

public class Auditable : EntityBase, IAuditable
{
    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset? UpdatedOn { get; set; }

    public Guid? CreatedById { get; set; }

    public User? CreatedBy { get; set; }

    public Guid? UpdatedById { get; set; }

    public User? UpdatedBy { get; set; }

}
