using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Helpers
{
    public class SpecificUser : ISpecificUser
    {
        private readonly UserManager<AppUser> _userManager;

        public SpecificUser(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<AppUser> GetSpecificUser(string email)
        {
            return  await _userManager.Users.Where(x => x.Email == email)
                .Include(x=>x.Role)
                .FirstOrDefaultAsync();
        }
    }
}