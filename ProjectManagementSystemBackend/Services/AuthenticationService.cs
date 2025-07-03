using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Azure.Core.HttpHeader;

namespace ProjectManagementSystemBackend.Services
{
    public class AuthenticationService : IAuthentication
    {
        private readonly string Issuer = "PMSBackend";
        private readonly string Key = "]VG#GgD6t0.x%GDc;yyz2Zi-v.En)NTBW]{5X_W!UE9O@;t/*~s73u8{xB7'";
        private SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

        public string GetJWT(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
            };

            var jwt = new JwtSecurityToken(
                issuer: Issuer,
                audience: user.Id.ToString(),
                expires: DateTime.UtcNow.AddMinutes(60),
                claims: claims,
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
           
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }




    }


}
}
