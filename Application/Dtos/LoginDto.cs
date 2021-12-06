using System.Collections.Generic;

namespace Application.Dtos
{
    public class LoginDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        
    }
}