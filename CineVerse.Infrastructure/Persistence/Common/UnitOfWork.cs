
using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Repositories;
using CineVerse.Infrastructure.Persistence.Contexts;
using CineVerse.Infrastructure.Persistence.Repositories;

namespace CineVerse.Infrastructure.Persistence.Common;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IGenreRepository Genres { get; }
    public IMovieRepository Movies { get; }
    public IMovieTagRepository MovieTags { get; }
    public IReviewRepository Reviews { get; }
    public ITagRepository Tags{ get; }

    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Genres = new GenreRepository(_context);
        Movies = new MovieRepository(_context);
        MovieTags = new MovieTagRepository(_context);
        Reviews = new ReviewRepository(_context);
        Tags = new TagRepository(_context);
    }

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();


}
