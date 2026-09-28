using Microsoft.AspNetCore.Identity;

namespace CineVerse.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DoB { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public ICollection<Review> Reviews { get; set; } = [];
    public AppUser()
    {
        Id = Guid.CreateVersion7(TimeProvider.System.GetUtcNow());
        CreatedOn = TimeProvider.System.GetUtcNow();
        SecurityStamp = Guid.CreateVersion7(TimeProvider.System.GetUtcNow()).ToString();
    }
}
