
using CineVerse.Application.Contracts.Repositories;
using CineVerse.Domain.Entities;
using CineVerse.Infrastructure.Persistence.Common;
using CineVerse.Infrastructure.Persistence.Contexts;

namespace CineVerse.Infrastructure.Persistence.Repositories;

public class MovieTagRepository : Repository<MovieTag>, IMovieTagRepository
{
    public MovieTagRepository(AppDbContext context) : base(context) { }
}
