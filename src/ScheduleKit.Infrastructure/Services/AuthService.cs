using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ScheduleKit.Application.Common.Interfaces;

namespace ScheduleKit.Infrastructure.Services;

/// <summary>
/// JWT authentication service implementation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly JwtSettings _settings;

    public AuthService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string HashPassword(string password)
    {
        // Using BCrypt-style hashing with PBKDF2
        using var deriveBytes = new Rfc2898DeriveBytes(
            password,
            saltSize: 16,
            iterations: 100000,
            HashAlgorithmName.SHA256);

        var salt = deriveBytes.Salt;
        var hash = deriveBytes.GetBytes(32);

        // Combine salt and hash
        var combined = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

        return Convert.ToBase64String(combined);
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            var combined = Convert.FromBase64String(storedHash);

            // Extract salt (first 16 bytes)
            var salt = new byte[16];
            Buffer.BlockCopy(combined, 0, salt, 0, 16);

            // Extract stored hash
            var storedHashBytes = new byte[32];
            Buffer.BlockCopy(combined, 16, storedHashBytes, 0, 32);

            // Hash the provided password with the same salt
            using var deriveBytes = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations: 100000,
                HashAlgorithmName.SHA256);

            var computedHash = deriveBytes.GetBytes(32);

            // Constant-time comparison
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
        }
        catch
        {
            return false;
        }
    }

    public string GenerateToken(Guid userId, string email, string name)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public Guid? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);

            return userId;
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// JWT configuration settings.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ScheduleKit";
    public string Audience { get; set; } = "ScheduleKit";
    public int ExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
