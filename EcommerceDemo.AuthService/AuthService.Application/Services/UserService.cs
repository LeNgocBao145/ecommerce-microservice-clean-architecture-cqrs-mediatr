using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Application.Services
{
    public class UserService(IUserRepository userRepository,
        IConfiguration config, IMapper mapper) : IUserService
    {
        public async Task<UserResponseDTO> GetUserById(Guid userId)
        {
            var user = await userRepository.GetByAsync(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }

            return mapper.Map<UserResponseDTO>(user);
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await userRepository.GetByAsync(u => u.Email == loginRequestDTO.Email);
            if (user == null)
            {
                return null;
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, user.HashedPassword);

            if (!isPasswordValid)
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return mapper.Map<LoginResponseDTO>(token);
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(secretKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: config.GetSection("Authentication:Issuer").Value,
                audience: config.GetSection("Authentication:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(240),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserResponseDTO> Register(UserRequestDTO userRequestDTO)
        {
            var existingUser = await userRepository.GetByAsync(u => u.Email == userRequestDTO.Email);

            if (existingUser != null)
            {
                return null;
            }

            var newUser = mapper.Map<User>(userRequestDTO);

            newUser.HashedPassword = BCrypt.Net.BCrypt.HashPassword(userRequestDTO.Password);

            var createdUser = await userRepository.CreateAsync(newUser);

            return mapper.Map<UserResponseDTO>(createdUser);
        }
    }
}
