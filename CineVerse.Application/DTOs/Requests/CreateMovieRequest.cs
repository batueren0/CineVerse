
namespace CineVerse.Application.DTOs.Requests;

public sealed record CreateMovieRequest(
    Guid GenreId,
    string Title,
    string Director,
    int ReleaseYear,
    string? PosterUrl,
    string Overview,
    bool IsInTheaters
    );
