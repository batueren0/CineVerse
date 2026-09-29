
using CineVerse.Application.Contracts.Repositories;
using CineVerse.Domain.Entities;
using CineVerse.Infrastructure.Persistence.Common;
using CineVerse.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CineVerse.Infrastructure.Persistence.Repositories;

public class TagRepository : Repository<Tag> , ITagRepository
{
    private readonly AppDbContext _context;
    public TagRepository(AppDbContext context) : base(context) 
    {
        _context = context;
    }

    public async Task<bool> AnyByNameAsync(string name, Guid? excludeId = null)
    {
        return await _context.Tags.AnyAsync(t => t.Name == name && (excludeId == null || t.Id != excludeId));
    }
}
