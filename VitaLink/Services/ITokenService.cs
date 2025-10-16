// ملف: Services/ITokenService.cs
using Dtos;
using Vitalink.Models;

namespace Vitalink.API.Services
{
    public class TokenResult
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
    }

    public interface ITokenService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        TokenResult GenerateAccessToken(AthleteProfile athlete);
        TokenResult GenerateRefreshToken(AthleteProfile athlete);
        Task SaveRefreshTokenAsync(AthleteProfile athlete, TokenResult refreshToken);
        TokenResponseDto CreateTokenResponseDto(AthleteProfile athlete, TokenResult accessToken, TokenResult refreshToken);
    }
}