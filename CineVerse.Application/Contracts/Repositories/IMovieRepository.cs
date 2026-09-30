using CineVerse.Application.Common;
using CineVerse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.Contracts.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    Task<bool> AnyByTitleAsync(string title, Guid? excludeId = null);
    Task<IEnumerable<Movie>> GetCarouselAsync();
    Task<IEnumerable<Movie>> GetRecentAsync();
    Task<IEnumerable<Movie>> GetAllByGenreIdAsync(Guid id);
    Task<IEnumerable<Movie>> GetAllByTagIdAsync(Guid id);
    Task<IEnumerable<Movie>> GetInTheatersAsync();
    Task<Movie?> GetByIdForUpdateAsync(Guid id);
    Task<bool> AnyByGenreIdAsync(Guid genreId);

}
