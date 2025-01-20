using Microsoft.EntityFrameworkCore;
using Server.DataAccess.Model;

namespace Server.DataAccess
{
    public class Db : DbContext
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = Policies.Admin },
                new Role { Id = 2, Name = Policies.StoreWorker },
                new Role { Id = 3, Name = Policies.Customer }
                );
        }
    }
}
