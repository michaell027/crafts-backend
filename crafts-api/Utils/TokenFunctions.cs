using crafts_api.models.dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace crafts_api.utils
{
    public class TokenFunctions
    {
        private readonly IConfiguration _configuration;

        public TokenFunctions(IConfiguration configuration) => _configuration = configuration;

        public string CreateToken(UserDto userDto)
        {
            List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userDto.FirstName!),
            new Claim(ClaimTypes.Surname, userDto.LastName!),
            new Claim(ClaimTypes.Email, userDto.Email!),
            new Claim(ClaimTypes.NameIdentifier, userDto.PublicId.ToString())
        };

            var key = _configuration["Jwt:Key"];
            var keyBytes = Encoding.UTF8.GetBytes(key!);
            var base64EncodedKey = Convert.ToBase64String(keyBytes);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(base64EncodedKey));

            var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
