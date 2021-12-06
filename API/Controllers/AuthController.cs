using System.Threading.Tasks;
using Application.Dtos;
using Application.Features.Auth.Commands.Handlers;
using Application.Features.Auth.Commands.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuthController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoginDto>> Login(LoginCommand command)
        {
            return await Mediator.Send(command);
        }
        
        [HttpPost("refreshToken")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginDto>> RefreshJwt(RefreshJwtTokenCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost("logout")]
        public async Task<ActionResult<bool>> Logout(LogoutCommand command)
        {
            return await Mediator.Send(command);
        }
        
    }
}