
using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Application.DTOs.Responses;

namespace CineVerse.Application.Contracts.Services;

public interface ITagService
{
    Task<Result> CreateAsync(CreateTagRequest request);
    Task<Result> UpdateAsync(UpdateTagRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<TagResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<TagResponse>> GetAllAsync();
}
