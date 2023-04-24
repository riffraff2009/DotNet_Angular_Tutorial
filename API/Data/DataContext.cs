
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser>  Users { get; set; }

        public DbSet<UserFollows> Follows { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserFollows>()
            .HasKey(k => new {k.SourceUserId, k.TargetUserId});

            builder.Entity<UserFollows>()
            .HasOne(s => s.SourceUser )
            .WithMany(l => l.FollowedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<UserFollows>()
            .HasOne(s => s.TargetUser )
            .WithMany(l => l.FollowedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
        }    
    }
}