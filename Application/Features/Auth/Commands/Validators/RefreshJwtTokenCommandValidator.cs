using Application.Features.Auth.Commands.Handlers;
using Application.Features.Auth.Commands.RequestModels;
using FluentValidation;

namespace Application.Features.Auth.Commands.Validators
{
    public class RefreshJwtTokenCommandValidator : AbstractValidator<RefreshJwtTokenCommand>
    {
        public RefreshJwtTokenCommandValidator()
        {
            /*RuleFor(x => x.TokenRequest.Token).NotEmpty();
            RuleFor(x => x.TokenRequest.RefreshToken).NotEmpty();*/
        }
    }
}