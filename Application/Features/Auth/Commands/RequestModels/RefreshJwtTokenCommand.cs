using Application.Dtos;
using MediatR;

namespace Application.Features.Auth.Commands.RequestModels
{
    public class RefreshJwtTokenCommand : IRequest<LoginDto>
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}