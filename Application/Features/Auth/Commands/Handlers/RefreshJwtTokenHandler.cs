using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Features.Auth.Commands.RequestModels;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.Auth.Commands.Handlers
{

    public class RefreshJwtTokenHandler : IRequestHandler<RefreshJwtTokenCommand, LoginDto>
    {
        private readonly DataContext _context;
        private readonly IJwtGenerator _jwtGenerator;

        public RefreshJwtTokenHandler(DataContext context, 
            IJwtGenerator jwtGenerator)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
        }
            
        public async Task<LoginDto> Handle(RefreshJwtTokenCommand request, CancellationToken cancellationToken)
        {
            // validation 1 - verify that the request refresh token exists on DB
            var storedToken = await _context.RefreshTokens
                .Include(x=>x.User)
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);
               
            if (storedToken == null) throw new WebException("Token is not valid (DB)",
                (WebExceptionStatus) HttpStatusCode.BadRequest);
            if (storedToken.Invalidated)  throw new WebException("Token is not valid (Invalidated)", 
                (WebExceptionStatus) HttpStatusCode.BadRequest);
                
            // Validation 2 - Verify if still valid
            var tokenIsRefreshTokenIsValid = await _jwtGenerator.VerifyTokenValidity(
                new TokenRequest
            {
                Token = request.Token,
                RefreshToken = request.RefreshToken
                
            }, storedToken);
            if (!tokenIsRefreshTokenIsValid) throw new WebException("Token is not valid",
                (WebExceptionStatus) HttpStatusCode.BadRequest);
                
                
            var refreshedToken =  await _jwtGenerator.GenerateToken(storedToken.User, true);
            return new LoginDto
            {
                Token = refreshedToken.Token,
                RefreshToken = refreshedToken.RefreshToken?.Token,
                Email = storedToken.User?.Email,
                Username = storedToken.User?.UserName,
                FullName = storedToken.User?.FullName
                
            };
                
        }
    }

}