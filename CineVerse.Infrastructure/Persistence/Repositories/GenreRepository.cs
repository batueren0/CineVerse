
using CineVerse.Application.Contracts.Repositories;
using CineVerse.Domain.Entities;
using CineVerse.Infrastructure.Persistence.Common;
using CineVerse.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CineVerse.Infrastructure.Persistence.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    private readonly AppDbContext _context;
    public GenreRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> AnyByNameAsync(string name, Guid? excludeId = null)
    {
        return await _context.Genres.AnyAsync(g => g.Name == name && (excludeId == null || g.Id != excludeId));
    }
}
