
using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using FluentValidation;

namespace CineVerse.Application.Validators;

public class UpdateTagValidator : AbstractValidator<UpdateTagRequest>
{
    private readonly IUnitOfWork _uow;
    public UpdateTagValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 100)
            .MustAsync(BeAnUniqueName)
            .WithMessage("This tag already exists");
    }

    private async Task<bool> BeAnUniqueName(UpdateTagRequest request ,string name, CancellationToken cancellationToken)
    {
        if (await _uow.Tags.AnyByNameAsync(name, request.Id))
            return false;

        return true;
    }
}
