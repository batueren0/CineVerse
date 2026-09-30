using CineVerse.Application.Common;
using CineVerse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.Contracts.Repositories;

public interface IMovieTagRepository : IRepository<MovieTag>
{
    Task<bool> AnyAsync(Guid movieId, Guid tagId);
    Task<MovieTag?> GetByMovieAndTagAsync(Guid movieId, Guid tagId);
}
