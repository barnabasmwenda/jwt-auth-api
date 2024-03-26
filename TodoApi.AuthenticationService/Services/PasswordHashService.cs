namespace TodoApi.AuthenticationService.Services
{
    public class PasswordHashService
    {
        private const int WorkFactor = 12;

        public bool VerifyPasswordHash(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }
    }
}
