using CineVerse.Domain.Common;

namespace CineVerse.Domain.Entities;

public class Movie : BaseEntity
{
    public Guid GenreId { get; set; }
    public Genre? Genre { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string? PosterUrl { get; set; }
    public string Overview { get; set; } = string.Empty;
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<MovieTag> Tags { get; set; } = [];
}
