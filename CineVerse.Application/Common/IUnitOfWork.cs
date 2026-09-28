using CineVerse.Application.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.Common;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenreRepository Genres { get; }
    IMovieRepository Movies { get; }
    IMovieTagRepository MovieTags { get; }
    IReviewRepository Reviews { get; }
    ITagRepository Tags { get; }


    Task<int> SaveChangesAsync();
}
