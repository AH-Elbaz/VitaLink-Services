namespace Dtos
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public int Role { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
    }
}