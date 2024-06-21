using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TodoApi.AuthenticationService.Data;
using TodoApi.AuthenticationService.Interfaces;
using TodoApi.AuthenticationService.Models;

namespace TodoApi.AuthenticationService.Services
{
    public class UserService : IUserService
    {
        private readonly TodoAuthenticationServiceDBContext _dbContext;

        private readonly PasswordHashService _passwordHashService;

        private readonly JwtTokenService _jwtTokenService;

        public UserService(TodoAuthenticationServiceDBContext dbContext, PasswordHashService passwordHashService, JwtTokenService jwtTokenService)
        {
            _dbContext = dbContext;

            _passwordHashService = passwordHashService;

            _jwtTokenService = jwtTokenService;
        }

        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            if (await _dbContext.Users.AnyAsync(u => u.Name == username))
                throw new ArgumentException("Username is already taken", nameof(username));

            if (IsValidEmail(email))
            {
                if (await _dbContext.Users.AnyAsync(u => u.Email == email))
                    throw new ArgumentException("Email is already taken", nameof(email));
            }
            else
            {
                throw new ArgumentException("Invalid email address", nameof(email));
            }

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long", nameof(password));

            var passwordIsValid = HasRequiredPasswordCharacters(password);

            if (!passwordIsValid)
                throw new ArgumentException("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character", nameof(password));

            string passwordHash = _passwordHashService.HashPassword(password);

            var user = new User { Name = username, Email = email, PasswordHash = passwordHash };

            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<string> AuthenticateAsync(string email, string password)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user != null && _passwordHashService.VerifyPasswordHash(password, user.PasswordHash))
            {
                return _jwtTokenService.GenerateJwtToken(user);
            }
            else
            {
                if (user == null)
                {
                    return "Invalid email address";
                }
                else
                {
                    return "Invalid password";
                }
            }
        }


        private bool IsValidEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                           + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<=[-a-z0-9])@"
                           + @"([a-z0-9][a-z0-9-]{0,61}[a-z0-9]\.?)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]\.com$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }

        private bool HasRequiredPasswordCharacters(string password)
        {
            var hasUppercase = new Regex(@"[A-Z]").IsMatch(password);

            var hasLowercase = new Regex(@"[a-z]").IsMatch(password);

            var hasDigit = new Regex(@"[0-9]").IsMatch(password);

            var hasSpecialChar = new Regex(@"[^A-Za-z0-9]").IsMatch(password);

            return hasUppercase && hasLowercase && hasDigit && hasSpecialChar;
        }
    }
}
