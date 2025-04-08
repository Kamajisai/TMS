using Microsoft.IdentityModel.Tokens;
using PtojectAPI.Datas;
using PtojectAPI.Entitys;
using PtojectAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PtojectAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        private readonly DBContext _context;

        public JwtService(IConfiguration config , DBContext context)
        {
            this._config = config;
            this._context = context;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("nameid", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task CleanupExpiredTokens()
        {
            var expiredTokens = _context.RefreshTokens.Where(rt => rt.ExpiryDate < DateTime.UtcNow);
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }

    }
}
