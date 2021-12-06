using Application.Dtos;
using MediatR;

namespace Application.Features.Auth.Commands.RequestModels
{
    public class LoginCommand : IRequest<LoginDto>
    {
        public string Email { get; set; }
        public string Password { get; set; } 
    }
}