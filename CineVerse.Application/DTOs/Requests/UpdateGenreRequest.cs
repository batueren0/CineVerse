
namespace CineVerse.Application.DTOs.Requests;

public sealed record UpdateGenreRequest(
    Guid Id,
    string Name
    );
