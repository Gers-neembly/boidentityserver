using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Neembly.BOIDServer.Persistence.Entities;

namespace Neembly.BOIDServer.Persistence
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<BackOfficeUser> BackOfficeUsers { get; set; }
        public DbSet<OperatorData> OperatorData { get; set; }
        public DbSet<OperatorAssignment> OperatorAssignments { get; set; }

    }
}
