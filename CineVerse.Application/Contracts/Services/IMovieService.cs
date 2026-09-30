using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.Contracts.Services;

public interface IMovieService
{
    Task<Result> CreateAsync(CreateMovieRequest request);
    Task<Result> UpdateAsync(UpdateMovieRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<MovieResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<MovieResponse>> GetAllAsync();
    Task<IEnumerable<MovieResponse>> GetCarouselAsync();
    Task<IEnumerable<MovieResponse>> GetRecentAsync();
    Task<IEnumerable<MovieResponse>> GetAllByGenreIdAsync(Guid id);
    Task<IEnumerable<MovieResponse>> GetAllByTagIdAsync(Guid id);
    Task<IEnumerable<MovieResponse>> GetInTheatersAsync();
}
