using FluentValidation;
using Nunari.Auth.Domain.Dtos.Requests;

namespace Nunari.Auth.Application.Validations;

public class LoginUserDtoValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
