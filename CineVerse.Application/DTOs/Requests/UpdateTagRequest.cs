
namespace CineVerse.Application.DTOs.Requests;

public sealed record UpdateTagRequest(
    Guid Id,
    string Name
    );
