using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services.Business
{
    public class PostService : IPostService
    {
        private readonly BlogDbContext _context;
        public PostService(BlogDbContext context)
        {
            _context = context;
        }
        public void Create(Post post)
        {
            try
            {
                _context.Posts.Add(post);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Post Get(int id)
        {
            var comment = _context.Comments.Where(x => x.PostId == id).Include(x => x.AppUser).ToList();
            var post = _context.Posts.Include(x => x.Reaction).Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);

            post.Comment = comment;

            return post;
        }

        public List<Post> GetAll()
        {
            var posts = _context.Posts.Include(p => p.Comment).Include(x => x.Reaction).Include(x => x.AppUser).ToList();

            return posts;
        }

        public Pagination<Post> GetPaginate(int pageIndex, int pageSize)
        {
            var posts = _context.Posts
                .Where(x => x.PostStatus == Status.Approve)
                .Include(p => p.Comment)
                .Include(x => x.Reaction)
                .Include(x => x.AppUser)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var pageData = new Pagination<Post>(posts, _context.Posts.Count(), pageIndex, pageSize);

            return pageData;
        }

        public void Update(Post post)
        {
            try
            {
                _context.Posts.Update(post);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
