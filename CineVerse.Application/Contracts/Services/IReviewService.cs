
using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Application.DTOs.Responses;

namespace CineVerse.Application.Contracts.Services;

public interface IReviewService
{
    Task<Result> CreateAsync(CreateReviewRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<IEnumerable<ReviewResponse>> GetAllAsync();
}
