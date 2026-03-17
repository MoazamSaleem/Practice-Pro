using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Practice_App.Models;

namespace Practice_Pro.Models
{
    public class DbCalls: IdentityDbContext
    {
        public DbCalls(DbContextOptions<DbCalls> options) : base(options)
        {
        }
        public DbSet<Client> client { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        public DbSet<UserRoleInfo> UserRolesInfo { get; set; }

    }
}
