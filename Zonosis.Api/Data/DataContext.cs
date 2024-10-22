using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zonosis.Shared.Entities;

namespace Zonosis.Api.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<UserFavorites> UserFavoritess { get; set; }
        public DbSet<UserAdoption> UserAdoptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserFavorites>()
            .HasKey(uf => new { uf.UserId, uf.PetId });
        }
    }
}
