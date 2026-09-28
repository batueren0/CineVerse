
using CineVerse.Application.Contracts.Repositories;
using CineVerse.Domain.Entities;
using CineVerse.Infrastructure.Persistence.Common;
using CineVerse.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CineVerse.Infrastructure.Persistence.Repositories;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    private readonly AppDbContext _context;
    public MovieRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    private IQueryable<Movie> MoviesWithDetails()
    {
        return _context.Movies
            .AsNoTracking()
            .AsSplitQuery()
            .Include(m => m.Genre)
            .Include(m => m.Tags)
                .ThenInclude(mt => mt.Tag)
            .Include(m => m.Reviews)
                .ThenInclude(r => r.User);
    }

    public async Task<bool> AnyByTitleAsync(string title)
    {
        return await _context.Movies.AnyAsync(m => m.Title == title);
    }

    public override async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await MoviesWithDetails()
            .OrderByDescending(m => m.CreatedOn)
            .ToListAsync();
    }

    public override async Task<Movie?> GetByIdAsync(Guid id)
    {
        return await MoviesWithDetails()
            .SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Movie>> GetAllByGenreIdAsync(Guid id)
    {
        return await MoviesWithDetails()
            .Where(m => m.GenreId == id)
            .OrderByDescending(m => m.CreatedOn)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetAllByTagIdAsync(Guid id)
    {
        return await MoviesWithDetails()
            .Where(m => m.Tags.Any(mt => mt.TagId == id))
            .OrderByDescending(m => m.CreatedOn)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetCarouselAsync()
    {
        return await MoviesWithDetails()
            .OrderByDescending(m => m.Reviews.Count)
            .Take(6)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetRecentAsync()
    {
        return await MoviesWithDetails()
            .OrderByDescending(m => m.CreatedOn)
            .Take(3)
            .ToListAsync();
    }
}
