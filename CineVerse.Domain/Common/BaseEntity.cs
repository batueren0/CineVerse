namespace CineVerse.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset LastModifiedOn { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }

    protected BaseEntity()
    {
        Id = Guid.CreateVersion7(TimeProvider.System.GetUtcNow());
        IsActive = true;
        CreatedOn = TimeProvider.System.GetUtcNow();
        LastModifiedOn = TimeProvider.System.GetUtcNow();
    }
}
