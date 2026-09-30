
using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using FluentValidation;

namespace CineVerse.Application.Validators;

public class UpdateMovieValidator : AbstractValidator<UpdateMovieRequest>
{
    private readonly IUnitOfWork _uow;
    public UpdateMovieValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.GenreId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(2, 256)
            .MustAsync(BeAnUniqueTitle)
            .WithMessage("This movie has already added");

        RuleFor(x => x.Director)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.ReleaseYear)
            .InclusiveBetween(1888 , DateTime.UtcNow.Year + 1);

        RuleFor(x => x.Overview)
            .NotEmpty()
            .MinimumLength(50);

    }

    private async Task<bool> BeAnUniqueTitle(UpdateMovieRequest request, string title , CancellationToken cancellationToken)
    {
        if (await _uow.Movies.AnyByTitleAsync(title, request.Id))
            return false;

        return true;
    }
}
