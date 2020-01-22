using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RPedretti.Grpc.Server.Helpers
{
    internal static class JwtHelper
    {
        public static string GenerateJwtToken(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Name is not specified.");
            }

            var claims = new[] { 
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, name == "admin" ? "Admin" : "User")
            };
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("ExampleServer", "ExampleClients", claims, expires: DateTime.Now.AddSeconds(60), signingCredentials: credentials);

            return JwtTokenHandler.WriteToken(token);
        }

        public static readonly JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();
        public static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());
    }
}
