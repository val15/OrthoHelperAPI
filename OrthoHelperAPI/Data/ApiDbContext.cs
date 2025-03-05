using Microsoft.EntityFrameworkCore;
using OrthoHelperAPI.Model;

namespace OrthoHelperAPI.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId);

        }
    }
}
