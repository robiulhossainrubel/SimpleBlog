using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Data
{
    public class BlogDbContext(DbContextOptions<BlogDbContext> options) : IdentityDbContext<AppUser, AppUserRole, int>(options)
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUserRole>().HasData(
                new AppUserRole
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new AppUserRole
                {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "User"
                }
                );
        }
    }
}