using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtService
{
    private readonly string _jwtKey;
    public JwtService(string jwtKey)
    {
        _jwtKey = jwtKey;
    }

    public string GenerateToken(Guid dealerId, string email)
    {
        //Storing dealerId inside claims
        var claims = new List<Claim>
        {
            new Claim("DealerId", dealerId.ToString()),
            new Claim(ClaimTypes.Email, email),
        };

        //Converting string key into bytes
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtKey)
            );

        //Creating signing credentials
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
            );

        //Creating actual token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}