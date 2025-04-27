using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services
{
    public class PostService : IPostService
    {
        private readonly BlogDbContext _context;

        public PostService(BlogDbContext context)
        {
            _context = context;
        }

        public void Create(PostDTO postDTO)
        {
            try
            {
                var post = new Post
                {
                    Title = postDTO.Title,
                    Body = postDTO.Body,
                    AppUserId = postDTO.AppUserId
                };

                _context.Posts.Add(post);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Post Get(int id)
        {
            try
            {
                var post = _context.Posts.Include(x => x.Reaction).Include(x => x.AppUser).Include(x => x.Comment).ThenInclude(x => x.AppUser).FirstOrDefault(x => x.Id == id);

                return post;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Post> GetAll(Expression<Func<Post, bool>>? expression = null)
        {
            try
            {
                if (expression == null)
                {
                    var posts = _context.Posts.Include(p => p.Comment).Include(x => x.Reaction).Include(x => x.AppUser).OrderByDescending(p => p.CreatedAt).ToList();

                    return posts;
                }
                else
                {
                    var posts = _context.Posts.Where(expression).Include(p => p.Comment).Include(x => x.Reaction).Include(x => x.AppUser).OrderByDescending(p => p.CreatedAt).ToList();

                    return posts;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Pagination<Post> GetPaginate(int pageIndex, int pageSize)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }

        public List<Post> TopPosts()
        {
            try
            {
                var posts = _context.Posts
                    .Include(p => p.Comment)
                    .Include(x => x.Reaction)
                    .Include(x => x.AppUser)
                    .OrderByDescending(x => x.Reaction.Count() + x.Comment.Count())
                    .Take(5)
                    .ToList();

                return posts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Post post)
        {
            try
            {
                _context.Posts.Update(post);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
