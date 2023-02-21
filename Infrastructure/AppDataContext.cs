using Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDataContext : IdentityDbContext<ApplicationUser>
    {
        public AppDataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Family> Family { get; set; }
        public DbSet<UserEvent> UserEvent { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<ApplicationUser> User { get; set; }
    }
}