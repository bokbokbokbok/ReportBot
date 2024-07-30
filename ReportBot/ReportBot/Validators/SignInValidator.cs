using FluentValidation;
using ReportBot.Common.Requests;

namespace ReportBot.Validators
{
    public class SignInValidator : AbstractValidator<SignInRequest>
    {
        public SignInValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Your email is invalid");
        }
    }
}
