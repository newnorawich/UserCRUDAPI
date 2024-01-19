using Microsoft.EntityFrameworkCore;

namespace Infranstructure
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserAuth> UserAuth { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> option): base(option) { }
    }
}
