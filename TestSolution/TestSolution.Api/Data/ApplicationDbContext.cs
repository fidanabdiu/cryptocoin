using Microsoft.EntityFrameworkCore;
using TestSolution.Api.Data.Configurations;
using TestSolution.Api.Data.Entities;

namespace TestSolution.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<CryptoCoin> CryptoCoin { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserCryptoCoin> UserCryptoCoin { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CryptoCoinConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserCryptoCoinConfiguration());
        }
    }
}