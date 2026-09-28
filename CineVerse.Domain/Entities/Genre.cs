using CineVerse.Domain.Common;

namespace CineVerse.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ICollection<Movie> Movies { get; set; } = [];
}
