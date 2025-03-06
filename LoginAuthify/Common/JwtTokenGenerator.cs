using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginAuthify.Common
{
    public static class JwtTokenGenerator
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GenerateToken(string username)
        {
            if (_configuration == null)
            {
                throw new InvalidOperationException("JwtTokenGenerator is not initialized. Call Initialize() first.");
            }

            string secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new Exception("JWT SecretKey is missing in configuration.");
            secretKey = EnsureKeySize(secretKey, 16); // Ensure key is at least 16 characters

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), // Token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string EnsureKeySize(string key, int minSize)
        {
            if (key.Length >= minSize)
            {
                return key;
            }

            return key.PadRight(minSize, 'X'); // Pad the key with 'X' if it's too short
        }
    }
}
