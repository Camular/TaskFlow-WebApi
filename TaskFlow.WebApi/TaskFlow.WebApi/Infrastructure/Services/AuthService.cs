using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.WebApi.Core.DTOs.Auth;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;

namespace TaskFlow.WebApi.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var trimmedUsername = request.Username.Trim(); 

            bool isUsernameExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == trimmedUsername.ToLower());

            if (isUsernameExists)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            bool isEmailExists = await _context.Users
                .AnyAsync(u => u.Email == normalizedEmail);

            if (isEmailExists)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = trimmedUsername,  
                Email = normalizedEmail,     
                PasswordHash = hashedPassword,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var expireMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "10080");
            var expireAt = DateTime.UtcNow.AddMinutes(expireMinutes);
            var token = GenerateJwtToken(user, expireAt);

            return new AuthResponse
            {
                UserId = user.Id,
                UserName = user.Username,
                Email = user.Email,
                Token = token,
                ExpireAt = expireAt
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var expireMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "10080");
            var expireAt = DateTime.UtcNow.AddMinutes(expireMinutes);
            var token = GenerateJwtToken(user, expireAt);

            return new AuthResponse
            {
                UserId = user.Id,
                UserName = user.Username,
                Email = user.Email,
                Token = token,
                ExpireAt = expireAt
            };
        }

        private string GenerateJwtToken(User user, DateTime expiresAt)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
