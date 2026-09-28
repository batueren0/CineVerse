using Microsoft.AspNetCore.Identity;

namespace CineVerse.Domain.Entities;

public class AppRole : IdentityRole<Guid>
{
    public DateTimeOffset CreatedOn { get; set; }

    public AppRole()
    {
        Id = Guid.CreateVersion7(TimeProvider.System.GetUtcNow());
        CreatedOn = TimeProvider.System.GetUtcNow();
    }
}
