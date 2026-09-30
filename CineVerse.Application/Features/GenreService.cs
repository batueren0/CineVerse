
using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Application.DTOs.Responses;
using FluentValidation;

namespace CineVerse.Application.Features;

public class GenreService(
    IUnitOfWork uow,
    IValidator<CreateGenreRequest> createValidator,
    IValidator<UpdateGenreRequest> updateValidator
    ) : IGenreService
{
    public async Task<Result> CreateAsync(CreateGenreRequest request)
    {
        var validationResult = await createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)));

        await uow.Genres.AddAsync(new()
        {
            Name = request.Name,
            Slug = request.Name.ToKebabCase()
        });

        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(UpdateGenreRequest request)
    {
        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)));

        var genre = await uow.Genres.GetByIdAsync(request.Id);
        if (genre is null)
            return Result.Failure("Genre not found");

        genre.Name = request.Name;
        genre.Slug = request.Name.ToKebabCase();
        genre.LastModifiedOn = DateTime.UtcNow;

        uow.Genres.Update(genre);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var genre = await uow.Genres.GetByIdAsync(id);
        if (genre is null)
            return Result.Failure("Genre not found");

        if (await uow.Movies.AnyByGenreIdAsync(id))
            return Result.Failure("This genre has movies. Move them to another genre before deleting it.");
        
        genre.IsActive = false;
        genre.DeletedOn = DateTime.UtcNow;

        uow.Genres.Delete(genre);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<GenreResponse?> GetByIdAsync(Guid id)
    {
        var genre = await uow.Genres.GetByIdAsync(id);
        if (genre is null)
            return default;

        return new GenreResponse(
            genre.Id,
            genre.Name,
            genre.Slug
            );
    }


    public async Task<IEnumerable<GenreResponse>> GetAllAsync()
    {
        var genres = await uow.Genres.GetAllAsync();
        if (!genres.Any())
            return [];

        return genres.Select(g => new GenreResponse(
            g.Id,
            g.Name,
            g.Slug
            )).ToList();
    }


}
