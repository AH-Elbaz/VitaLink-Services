using BCrypt.Net;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Vitalink.Models;
using VitaLink.Models.Data;

namespace Vitalink.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly VitalinkDbContext _context;

        public TokenService(IConfiguration config, VitalinkDbContext context)
        {
            _config = config;
            _context = context;
        }

        // ----------------------------------------------------
        // 1 & 2: تشفير كلمة المرور (Hashing & Verification)
        // ----------------------------------------------------
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        // ----------------------------------------------------
        // 3: توليد Access Token (JWT)
        // ----------------------------------------------------
        public TokenResult GenerateAccessToken(AthleteProfile athlete)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, athlete.AthleteID),
                new Claim(ClaimTypes.Name, athlete.FirstName),
                new Claim(ClaimTypes.Role, athlete.Role.ToString())
            };

            // انتهاء الصلاحية: 15 دقيقة
            var expires = DateTime.UtcNow.AddMinutes(15);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new TokenResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiryDate = expires
            };
        }

        // ----------------------------------------------------
        // 4: توليد Refresh Token (سلسلة عشوائية)
        // ----------------------------------------------------
        public TokenResult GenerateRefreshToken(AthleteProfile athlete)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            // انتهاء الصلاحية: 7 أيام
            var expires = DateTime.UtcNow.AddDays(7);

            return new TokenResult
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiryDate = expires
            };
        }

        // ----------------------------------------------------
        // 5: حفظ Refresh Token في قاعدة البيانات
        // ----------------------------------------------------
        public async Task SaveRefreshTokenAsync(AthleteProfile athlete, TokenResult refreshToken)
        {
            // 1. إزالة أي توكنات تجديد قديمة لنفس المستخدم
            var existingTokens = _context.RefreshTokens
                .Where(t => t.AthleteID == athlete.AthleteID);
            _context.RefreshTokens.RemoveRange(existingTokens);

            // 2. إنشاء التوكن الجديد
            var newRefreshToken = new RefreshToken
            {
                Token = refreshToken.Token,
                ExpiryDate = refreshToken.ExpiryDate,
                AthleteID = athlete.AthleteID
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------------------
        // 6 & 7: التحقق من الصلاحية وإنشاء DTO (تم ترك 6 كواجب منزلي)
        // ----------------------------------------------------

        // 7. تحويل النتيجة النهائية إلى DTO
        public TokenResponseDto CreateTokenResponseDto(AthleteProfile athlete, TokenResult accessToken, TokenResult refreshToken)
        {
            return new TokenResponseDto
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                FirstName = athlete.FirstName,
                Role = athlete.Role,
                AccessTokenExpiry = accessToken.ExpiryDate
            };
        }

        // ** ملاحظة: تم ترك دالة ValidateRefreshTokenAsync غير مطبقة هنا **
        // هذا الجزء ضروري لـ /refresh-token endpoint ولكن يمكن إضافته لاحقاً.
        public Task<AthleteProfile?> ValidateRefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}