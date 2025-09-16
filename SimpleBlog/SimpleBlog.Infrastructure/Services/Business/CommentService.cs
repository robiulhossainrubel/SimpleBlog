using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services.Business
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<Comment> GetAll()
        {
            var comments = _context.Comments.Include(c => c.AppUser).ToList();

            return comments;
        }
    }
}
