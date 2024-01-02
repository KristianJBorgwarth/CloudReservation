using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudReservation.DAL.Entities;
using CloudReservation.Service.Wrappers.ConfigurationWrapper;
using Microsoft.IdentityModel.Tokens;

namespace CloudReservation.Service.Services.TokenServices;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _timeToLive;

    public JwtService(IConfigurationWrapper configWrapper)
    {
        _secretKey = configWrapper.GetJwtConfiguration<string>(ConfigurationType.Key);
        _issuer = configWrapper.GetJwtConfiguration<string>(ConfigurationType.Issuer);
        _audience = configWrapper.GetJwtConfiguration<string>(ConfigurationType.Audience);
        _timeToLive = configWrapper.GetJwtConfiguration<int>(ConfigurationType.TimeToLive);
    }

    public string GenerateToken(Employee employee)
    {
        var claims = employee.Claims.Select(userClaim => new Claim(userClaim.ClaimType, userClaim.ClaimValue)).ToList();

        claims.Add(new Claim(ClaimTypes.Email, employee.Email));

        var claimsIdentity = new ClaimsIdentity(claims);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddMinutes(_timeToLive),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}