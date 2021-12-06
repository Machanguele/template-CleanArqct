using MediatR;

namespace Application.Features.Auth.Commands.RequestModels
{
    public class LogoutCommand : IRequest<bool>
    {
    }
}