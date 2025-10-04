using ConsultaDocumentos.Application.DTOs;
using ConsultaDocumentos.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConsultaDocumentos.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            return GenerateJwtToken(user);
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO registerRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Email já está em uso");
            }

            var user = new IdentityUser
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Erro ao criar usuário: {errors}");
            }

            return GenerateJwtToken(user);
        }

        private AuthResponseDTO GenerateJwtToken(IdentityUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationHours = int.Parse(jwtSettings["ExpirationHours"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(expirationHours);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDTO
            {
                Token = tokenString,
                Email = user.Email,
                Expiration = expiration
            };
        }
    }
}
