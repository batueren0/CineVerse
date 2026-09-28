using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using FluentValidation;


namespace CineVerse.Application.Validators;

public class CreateGenreValidator : AbstractValidator<CreateGenreRequest>
{
    private readonly IUnitOfWork _uow;
    public CreateGenreValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 100)
            .MustAsync(BeAnUniqueName)
            .WithMessage("This genre already exists");
    }

    private async Task<bool> BeAnUniqueName(string name , CancellationToken cancellationToken)
    {
        if (await _uow.Genres.AnyByNameAsync(name))
            return false;

        return true;
    }
}
