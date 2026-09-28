using CineVerse.Domain.Common;

namespace CineVerse.Domain.Entities;

public class MovieTag : BaseEntity
{
    public Guid MovieId { get; set; }
    public Movie? Movie { get; set; }
    public Guid TagId { get; set; }
    public Tag? Tag { get; set; }
}
