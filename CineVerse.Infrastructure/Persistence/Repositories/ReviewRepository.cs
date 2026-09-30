
using CineVerse.Application.Contracts.Repositories;
using CineVerse.Domain.Entities;
using CineVerse.Infrastructure.Persistence.Common;
using CineVerse.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CineVerse.Infrastructure.Persistence.Repositories;

public class ReviewRepository : Repository<Review> , IReviewRepository
{
    private readonly AppDbContext _context;
    public ReviewRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await _context.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Movie)
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync();
    }
}
