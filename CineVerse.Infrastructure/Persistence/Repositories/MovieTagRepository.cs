
using CineVerse.Application.Contracts.Repositories;
using CineVerse.Domain.Entities;
using CineVerse.Infrastructure.Persistence.Common;
using CineVerse.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CineVerse.Infrastructure.Persistence.Repositories;

public class MovieTagRepository : Repository<MovieTag>, IMovieTagRepository
{
    public MovieTagRepository(AppDbContext context) : base(context) { }

    public async Task<bool> AnyAsync(Guid movieId, Guid tagId)
    {
        return await _dbSet.AnyAsync(mt => mt.MovieId == movieId && mt.TagId == tagId);
    }

    public async Task<MovieTag?> GetByMovieAndTagAsync(Guid movieId, Guid tagId)
    {
        return await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(mt => mt.MovieId == movieId && mt.TagId == tagId);
    }
}
