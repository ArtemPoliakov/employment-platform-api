using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    /// <summary>
    /// Service for creating JWT Tocken operations.
    /// </summary>
    public class TockenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly SymmetricSecurityKey _key;
        public TockenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }

        /// <summary>
        /// Creates a JSON Web Token (JWT) for the given application user.
        /// </summary>
        /// <param name="appUser">The application user for whom the token is being created.</param>
        /// <returns>A task representing the asynchronous operation, which, when completed, returns a JWT as a string.</returns>
        public async Task<string> CreateToken(AppUser appUser)
        {
            var roles = await _userManager.GetRolesAsync(appUser);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, appUser.UserName),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tockenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tockenHandler = new JwtSecurityTokenHandler();

            var tocken = tockenHandler.CreateToken(tockenDescriptor);

            return tockenHandler.WriteToken(tocken);
        }
    }
}