using crafts_api.exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using crafts_api.Entities.Dto;

namespace crafts_api.utils
{
    public class TokenFunctions
    {
        private readonly IConfiguration _configuration;

        public TokenFunctions(IConfiguration configuration) => _configuration = configuration;

        public string CreateToken(LoggedUserDto userDto)
        {
            IEnumerable<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userDto.FirstName!),
            new Claim(ClaimTypes.Surname, userDto.LastName!),
            new Claim(ClaimTypes.Email, userDto.Email!),
            new Claim(ClaimTypes.NameIdentifier, userDto.PublicId.ToString()),
            new Claim(ClaimTypes.Role, userDto.Role.ToString())
        };

            var key = _configuration["Jwt:Key"];
            var keyBytes = Encoding.UTF8.GetBytes(key!);
            var symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);

            var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(10),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
            //var token = new JwtSecurityToken(
            //    issuer: _configuration["Jwt:Issuer"],
            //    audience: _configuration["Jwt:Audience"],
            //    claims: claims,
            //    expires: DateTime.Now.AddSeconds(10),
            //    signingCredentials: creds);

            //return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var secret = _configuration["Jwt:Key"] ?? throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Key not found"
            };

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ClockSkew = TimeSpan.Zero
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out _);
        }

        //get data from token
        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var stringClaimValue = securityToken?.Claims.First(claim => claim.Type == claimType).Value;

            if (stringClaimValue == null)
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = 400,
                    Message = "Claim not found in token"
                };
            }

            return stringClaimValue;
        }
    }

}
