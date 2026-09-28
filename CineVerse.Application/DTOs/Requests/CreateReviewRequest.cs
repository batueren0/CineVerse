
namespace CineVerse.Application.DTOs.Requests;

public sealed record CreateReviewRequest(
    Guid MovieId,
    int Rating,
    string Body
    )
{
    public Guid UserId { get; set; }
}
