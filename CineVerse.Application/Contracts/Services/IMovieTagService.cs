
using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;

namespace CineVerse.Application.Contracts.Services;

public interface IMovieTagService
{
    Task<Result> CreateAsync(CreateMovieTagRequest request);
    Task<Result> DeleteAsync(Guid movieId, Guid tagId);
}
