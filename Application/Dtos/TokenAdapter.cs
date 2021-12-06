using Domain;

namespace Application.Dtos
{
    public class TokenAdapter
    {
        public string Token { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}