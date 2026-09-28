
using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using FluentValidation;

namespace CineVerse.Application.Validators;

public class UpdateMovieValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.GenreId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(2, 256);

        RuleFor(x => x.Director)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.ReleaseYear)
            .InclusiveBetween(1888 , DateTime.UtcNow.Year + 1);

        RuleFor(x => x.Overview)
            .NotEmpty()
            .MinimumLength(50);
    }
}
