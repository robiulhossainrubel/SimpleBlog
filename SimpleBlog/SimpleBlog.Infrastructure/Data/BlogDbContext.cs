using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Data
{
    public class BlogDbContext(DbContextOptions<BlogDbContext> options) : IdentityDbContext<AppUser, AppUserRole, int>(options)
    {

    }
}