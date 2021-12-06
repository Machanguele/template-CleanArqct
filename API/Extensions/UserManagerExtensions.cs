using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindUserByEmailAndClaimsPrincipalAsync(this UserManager<AppUser> input,
            ClaimsPrincipal user)
        {
            var email = user?.Claims?
                .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            return await input.Users
                .SingleOrDefaultAsync(x => x.Email == email);
        }

        public static async Task<AppUser> FindUserByUserNameAndClaimsPrincipalAsync(this UserManager<AppUser> input,
            ClaimsPrincipal user)
        {
            var email = user?.Claims?
                .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            return await input.Users
                .SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}