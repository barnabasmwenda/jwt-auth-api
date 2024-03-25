using Microsoft.EntityFrameworkCore;
using TodoApi.AuthenticationService.Models;

namespace TodoApi.AuthenticationService.Data
{
    public class TodoAuthenticationServiceDBContext : DbContext
    {
        public TodoAuthenticationServiceDBContext(DbContextOptions<TodoAuthenticationServiceDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
