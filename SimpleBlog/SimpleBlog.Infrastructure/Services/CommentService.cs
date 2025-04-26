using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly BlogDbContext _context;

        public CommentService(BlogDbContext context)
        {
            _context = context;
        }

        public void Create(Comment comment)
        {
            try
            {
                _context.Comments.Add(comment);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public List<Comment> GetAll()
        {
            try
            {
                var comments = _context.Comments.Include(c => c.AppUser).ToList();

                return comments;
            }
            catch
            {
                throw;
            }
        }
    }
}
