using CineVerse.Domain.Common;

namespace CineVerse.Domain.Entities;

public class Review : BaseEntity
{
    public Guid MovieId { get; set; }
    public Movie? Movie { get; set; }
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public int Rating { get; set; }
    public string Body { get; set; } = string.Empty;
}
