using CineVerse.Application.Common;
using CineVerse.Application.DTOs.Requests;
using FluentValidation;

namespace CineVerse.Application.Validators
{
    public class CreateTagValidator : AbstractValidator<CreateTagRequest>
    {
        private readonly IUnitOfWork _uow;
        public CreateTagValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 100)
                .MustAsync(BeAnUniqueName)
                .WithMessage("This tag already exists");
        }

        private async Task<bool> BeAnUniqueName(string name, CancellationToken cancellationToken)
        {
            if (await _uow.Tags.AnyByNameAsync(name))
                return false;

            return true;
        }
    }

    
}
