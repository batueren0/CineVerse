
using CineVerse.Application.DTOs.Requests;
using FluentValidation;

namespace CineVerse.Application.Validators;

public class CreateMovieTagValidator : AbstractValidator<CreateMovieTagRequest>
{
    public CreateMovieTagValidator()
    {
        RuleFor(x => x.MovieId)
            .NotEmpty();
        RuleFor(x => x.TagId)
            .NotEmpty();
    }
}
