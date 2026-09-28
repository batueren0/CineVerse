
namespace CineVerse.Application.DTOs.Requests;

public sealed record CreateMovieTagRequest(
    Guid MovieId,
    Guid TagId
    );
