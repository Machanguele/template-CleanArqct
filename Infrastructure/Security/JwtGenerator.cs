using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace Infrastructure
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly UserManager<AppUser> _userManager;
        private readonly SymmetricSecurityKey _key;

        public JwtGenerator(IConfiguration configuration, DataContext context, 
            TokenValidationParameters tokenValidationParams, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _context = context;
            _tokenValidationParams = tokenValidationParams;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:TokenSecret"]));

        }
        public async Task<TokenAdapter>  GenerateToken(AppUser user, bool updateExistingToken)
        {
            if (user == null) throw new WebException("User is Empty", (WebExceptionStatus) HttpStatusCode.BadRequest);
            var userExists = await _context.Users
                .FirstOrDefaultAsync(x=>x.Email == user.Email  && !user.Archived);
            if (userExists == null) throw new WebException("User not found or is Archived", 
                (WebExceptionStatus) HttpStatusCode.NotFound);
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Token:Duration"])),
                SigningCredentials = creds,

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JWtId = token.Id,
                User = user,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Token:Duration"])),
                Token = RandomString(35) + Guid.NewGuid()
            };
            
            if (!updateExistingToken)
                await _context.RefreshTokens.AddAsync(refreshToken);
            else {
                var storedRefreshToken = await _context.RefreshTokens
                    .Include(x=>x.User)
                    .FirstOrDefaultAsync(x => x.User.Email ==user.Email && !x.Invalidated);

                if (storedRefreshToken != null)
                {
                    storedRefreshToken.ExpiresAt = refreshToken.ExpiresAt;
                    storedRefreshToken.Token = refreshToken.Token;
                }
            }
            await _context.SaveChangesAsync();

            return new TokenAdapter
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };
        }
        
           public async Task<bool> VerifyTokenValidity(TokenRequest tokenRequest, RefreshToken refreshToken)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                //1st Validation, verify that the token has claims
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, 
                    _tokenValidationParams, out var validatedToken);

                //2nd validation, compare the encrypt algorithm and token algorithm are equals
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, 
                        StringComparison.InvariantCultureIgnoreCase);
                    if (result == false) return false;
                }
                
                if(refreshToken.ExpiresAt < DateTime.UtcNow) return false;
                
                return true;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new WebException("An error occured while validating refresh token");
            }
        }
           private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ADDDFSHFDDGFH12345FNVCXSAERRTGHJVNB";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
}