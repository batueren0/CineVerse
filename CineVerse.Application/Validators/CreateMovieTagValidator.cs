
using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using FluentValidation;

namespace CineVerse.Application.Validators;

public class CreateMovieTagValidator : AbstractValidator<CreateMovieTagRequest>
{
    private readonly IUnitOfWork _uow;
    public CreateMovieTagValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(x => x.MovieId)
            .NotEmpty();
        RuleFor(x => x.TagId)
            .NotEmpty();

        RuleFor(x => x)
            .MustAsync(NotBeAlreadyAdded)
            .WithMessage("This tag is already added to the movie.");
    }

    private async Task<bool> NotBeAlreadyAdded(CreateMovieTagRequest request, CancellationToken cancellationToken)
    {
        if (await _uow.MovieTags.AnyAsync(request.MovieId, request.TagId))
            return false;

        return true;
    }
}
