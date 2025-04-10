using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BugProject.DL;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BugProject.BL
{
    public class AccountManager : IAccountManager
    {
        
        private readonly IConfiguration config;
        private readonly UserManager<User> userManger;

        public AccountManager(UserManager<User> _userManger, IConfiguration _config)
        {
            
            config = _config;
            userManger = _userManger;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
        {
            User user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };
            return await userManger.CreateAsync(user, registerDto.Password);
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            User userFromDb = await userManger.FindByNameAsync(loginDto.UserName);
            if (userFromDb != null && await userManger.CheckPasswordAsync(userFromDb, loginDto.Password))
            {
                // Generate JWT Token
                List<Claim> userClaims = new List<Claim>
                {
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(ClaimTypes.NameIdentifier, userFromDb.Id.ToString()),
                 new Claim(ClaimTypes.Name, userFromDb.UserName)
                 
                };
                var userRoles = await userManger.GetRolesAsync(userFromDb);
                foreach (var role in userRoles)
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecritKey"]));
                var signingCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256);

                var jwtToken = new JwtSecurityToken(
                    audience: config["JWT:AudienceIP"],
                    issuer: config["JWT:IssuerIP"],
                    expires: DateTime.Now.AddHours(1),
                    claims: userClaims,
                    signingCredentials: signingCredentials
                );

                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }

            return null;
        }
    }
}
