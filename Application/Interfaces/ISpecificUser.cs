using System.Threading.Tasks;
using Domain;

namespace Application.Interfaces
{
    public interface ISpecificUser
    {
        Task<AppUser> GetSpecificUser(string email);
    }
}