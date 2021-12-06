using System;

namespace Domain
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string JWtId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
        public virtual AppUser User { get; set; }
    }
}