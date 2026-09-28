
using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Application.DTOs.Responses;

namespace CineVerse.Application.Contracts.Services;

public interface IGenreService
{
    Task<Result> CreateAsync(CreateGenreRequest request);
    Task<Result> UpdateAsync(UpdateGenreRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<GenreResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<GenreResponse>> GetAllAsync();
}
