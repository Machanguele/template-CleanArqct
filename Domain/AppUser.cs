using System;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public sealed class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public Role Role { get; set; }
        public bool LoggedIn { get; set; }
        public bool Archived { get; set; }
    }
}