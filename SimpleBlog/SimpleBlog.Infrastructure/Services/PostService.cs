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
            try
            {
                context.Posts.Add(post);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Post Get(int id)
        {
            var comment = context.Comments.Where(x => x.PostId == id).Include(x => x.AppUser).ToList();
            var post = context.Posts.Include(x => x.Reaction).Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);

            post.Comment = comment;

            return post;
        }

        public List<Post> GetAll()
        {
            var posts = context.Posts.Include(p => p.Comment).Include(x => x.Reaction).Include(x => x.AppUser).ToList();

            return posts;
        }

        public void Update(Post post)
        {
            try
            {
                context.Posts.Update(post);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
