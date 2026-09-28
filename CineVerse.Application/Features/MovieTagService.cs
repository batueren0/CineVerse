
using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Domain.Entities;
using FluentValidation;

namespace CineVerse.Application.Features;

public class MovieTagService(IUnitOfWork uow,
    IValidator<CreateMovieTagRequest> validator
    ) : IMovieTagService
{
    public async Task<Result> CreateAsync(CreateMovieTagRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)));

        var movieTag = new MovieTag
        {
            Id = Guid.CreateVersion7(),
            MovieId = request.MovieId,
            TagId = request.TagId,
        };

        await uow.MovieTags.AddAsync(movieTag);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var movieTag = await uow.MovieTags.GetByIdAsync(id);
        if(movieTag is null)
            return Result.Failure("Movie - Tag connection not found");


        movieTag.IsActive = false;
        movieTag.DeletedOn = DateTimeOffset.UtcNow;

        uow.MovieTags.Delete(movieTag);
        await uow.SaveChangesAsync();

        return Result.Success();
      
    }
}
