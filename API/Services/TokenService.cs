using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// SERVICE TO GENERATING JWT

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // this key is used to both encrypt and decrypt electronic certifications, in this case, sign and verify JWT
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config) // config reads from appsettings.json
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        // this method generates and return a JWT:
        public string CreateToken(AppUser user)
        {
            // here, we're binding both the user's id and username to the token:
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // userId
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName), // userName
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}