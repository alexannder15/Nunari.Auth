using Nunari.Auth.Domain.AggregateRoot;

namespace Nunari.Auth.Domain.Models.Common;

public interface IAuditable
{
    bool IsDeleted { get; set; }

    DateTimeOffset CreatedOn { get; set; }

    DateTimeOffset? UpdatedOn { get; set; }

    Guid? CreatedById { get; set; }

    User? CreatedBy { get; set; }

    Guid? UpdatedById { get; set; }

    User? UpdatedBy { get; set; }
}
