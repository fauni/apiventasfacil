using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSerie> UserSeries { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
