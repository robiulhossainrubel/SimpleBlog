using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services
{
    public class PostService(BlogDbContext context) : IPostService
    {
        public void Create(Post post)
        {
            context.Posts.Add(post);
            context.SaveChanges();
        }

        public Post Get(int id)
        {
            return context.Posts.Include(x => x.LikeDisLike).Include(x => x.Comment).Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);
        }

        public List<Post> GetAll()
        {
            var posts = context.Posts.Include(p => p.LikeDisLike).Include(p => p.Comment).Include(x => x.AppUser).ToList();

            return posts;
        }

        public void Update(Post post)
        {
            context.Update(post);
            context.SaveChanges();
        }
    }
}
