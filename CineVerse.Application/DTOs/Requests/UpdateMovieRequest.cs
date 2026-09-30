
namespace CineVerse.Application.DTOs.Requests;

public sealed record UpdateMovieRequest(
    Guid Id,
    Guid GenreId,
    string Title,
    string Director,
    int ReleaseYear,
    string? PosterUrl,
    string Overview,
    bool IsInTheaters
    );