using CineVerse.Domain.Common;

namespace CineVerse.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ICollection<MovieTag> Movies { get; set; } = [];
}
