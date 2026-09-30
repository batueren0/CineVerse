using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Application.DTOs.Responses;
using CineVerse.Domain.Entities;
using FluentValidation;

namespace CineVerse.Application.Features;

public class MovieService(IUnitOfWork uow,
    IValidator<CreateMovieRequest> createValidator,
    IValidator<UpdateMovieRequest> updateValidator
    ) : IMovieService
{
    public async Task<Result> CreateAsync(CreateMovieRequest request)
    {
        var validationResult = await createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)));

        var movie = new Movie
        {
            Id = Guid.CreateVersion7(),
            GenreId = request.GenreId,
            Title = request.Title,
            Slug = request.Title.ToKebabCase(),
            Director = request.Director,
            ReleaseYear = request.ReleaseYear,
            PosterUrl = request.PosterUrl,
            Overview = request.Overview,
            IsInTheaters = request.IsInTheaters,
        };

        await uow.Movies.AddAsync(movie);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(UpdateMovieRequest request)
    {
        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)));


        var movie = await uow.Movies.GetByIdForUpdateAsync(request.Id);
        if (movie == null)
            return Result.Failure("Movie was not found");

        movie.GenreId = request.GenreId;
        movie.Title = request.Title;
        movie.Slug = request.Title.ToKebabCase();
        movie.Director = request.Director;
        movie.ReleaseYear = request.ReleaseYear;
        movie.PosterUrl = request.PosterUrl;
        movie.Overview = request.Overview;
        movie.IsInTheaters = request.IsInTheaters;
        movie.LastModifiedOn = DateTimeOffset.UtcNow;

        uow.Movies.Update(movie);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var movie = await uow.Movies.GetByIdForUpdateAsync(id);
        if(movie is null)
            return Result.Failure($"{nameof(Movie)} does not exist");
        
       
        movie.IsActive = false;
        movie.DeletedOn = DateTimeOffset.UtcNow;

        uow.Movies.Delete(movie);
        await uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<MovieResponse?> GetByIdAsync(Guid id)
    {
        var movie = await uow.Movies.GetByIdAsync(id);
        if (movie is null)
            return default;

        return ToResponse(movie);
    }

    public async Task<IEnumerable<MovieResponse>> GetAllAsync()
    {
        var movies = await uow.Movies.GetAllAsync();
        return movies.Select(ToResponse);
    }


    public async Task<IEnumerable<MovieResponse>> GetCarouselAsync()
    {
        var movies = await uow.Movies.GetCarouselAsync();
        return movies.Select(ToResponse);
    }

    public async Task<IEnumerable<MovieResponse>> GetRecentAsync()
    {
        var movies = await uow.Movies.GetRecentAsync();
        return movies.Select(ToResponse);
    }

    public async Task<IEnumerable<MovieResponse>> GetAllByGenreIdAsync(Guid id)
    {
        var movies = await uow.Movies.GetAllByGenreIdAsync(id);
        return movies.Select(ToResponse);
    }


    public async Task<IEnumerable<MovieResponse>> GetAllByTagIdAsync(Guid id)
    {
        var movies = await uow.Movies.GetAllByTagIdAsync(id);
        return movies.Select(ToResponse);
    }

    public async Task<IEnumerable<MovieResponse>> GetInTheatersAsync()
    {
        var movies = await uow.Movies.GetInTheatersAsync();
        return movies.Select(ToResponse);
    }

    private static MovieResponse ToResponse(Movie movie)
    {
        return new MovieResponse
        (
            movie.Id,
            movie.Title,
            movie.Slug,
            movie.Director,
            movie.ReleaseYear,
            movie.PosterUrl,
            movie.Overview,
            movie.IsInTheaters,
            movie.LastModifiedOn,
            new GenreResponse(
                Id: movie.Genre!.Id,
                Name: movie.Genre.Name,
                Slug: movie.Genre.Slug),
            movie.Tags.Select(mt => new TagResponse(
                Id: mt.Tag!.Id,
                Name: mt.Tag.Name,
                Slug: mt.Tag.Slug
                )),
            movie.Reviews.Select(r => new ReviewResponse(
                r.Id,
                r.User != null ? r.User.UserName ?? string.Empty :
                    string.Empty,
                    r.Rating,
                    r.Body,
                    r.CreatedOn
                ))
        );
    }
}

