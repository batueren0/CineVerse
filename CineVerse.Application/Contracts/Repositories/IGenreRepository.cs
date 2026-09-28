using CineVerse.Application.Common;
using CineVerse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.Contracts.Repositories;

public interface IGenreRepository : IRepository<Genre>
{
    Task<bool> AnyByNameAsync(string name);
}
