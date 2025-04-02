using Microsoft.EntityFrameworkCore;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;

namespace OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext
{
    public class ApiDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration éventuelle du modèle
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId);
        }
    }
}
