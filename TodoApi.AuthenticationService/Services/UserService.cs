using Microsoft.EntityFrameworkCore;
using TodoApi.AuthenticationService.Data;
using TodoApi.AuthenticationService.Interfaces;
using TodoApi.AuthenticationService.Models;

namespace TodoApi.AuthenticationService.Services
{    
    public class UserService : IUserService
    {
        private readonly TodoAuthenticationServiceDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly PasswordHashService _passwordHashService;
        private readonly JwtTokenService _jwtTokenService;

        public UserService(TodoAuthenticationServiceDBContext dbContext, IConfiguration configuration, PasswordHashService passwordHashService, JwtTokenService jwtTokenService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _passwordHashService = passwordHashService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Name == username))
                throw new ArgumentException("Username is already taken", nameof(username));

            string passwordHash = _passwordHashService.HashPassword(password);

            var user = new User { Name = username, Email = email, PasswordHash = passwordHash };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Name == username);
            if (user == null || !_passwordHashService.VerifyPasswordHash(password, user.PasswordHash))
                return null;

            return _jwtTokenService.GenerateJwtToken(user);
        }
    }
}
