using AuthService.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("auth");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(x => x.PasswordHash)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.Property(x => x.RefreshToken)
                      .HasMaxLength(500);

                entity.Property(x => x.RefreshTokenExpiresAt);
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("Profiles");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.CreatedAt)
                      .IsRequired();
            });            

            modelBuilder.Entity<Profile>().HasData(
                new Profile
                {
                    Id = 1,
                    Name = "ADMINISTRADOR",
                    CreatedAt = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(x => x.Profile)
                      .WithMany()
                      .HasForeignKey(x => x.ProfileId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
