using FluentValidation;
using Nunari.Auth.Domain.Dtos.Requests;

namespace Nunari.Auth.Application.Validations;

public class CreateUserDtoValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}
