using Godot;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cassebrique.Services
{
    /// <summary>
    /// Authentication token manager for API
    /// </summary>
    public class AuthenticationTokenService : IAuthenticationTokenService
    {
        private JwtSecurityToken _token;

        /// <summary>
        /// Get Authentication token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            if (_token == null)
                GenerateToken();

            return new JwtSecurityTokenHandler().WriteToken(_token);
        }

        /// <summary>
        /// Generated token
        /// </summary>
        private void GenerateToken()
        {
            // Should be obtained from dedicated API or third party service
            var securityKey = new SymmetricSecurityKey(UTF8Encoding.UTF8.GetBytes("BrickBreakAuthoKey"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, "user_name"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            _token = new JwtSecurityToken("BrickBreakerApp",
                "BrickBreakerApp",
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            GD.Print(_token.ToString());
        }
    }
}
