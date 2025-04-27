using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.DTOs;
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

        public void Create(CommentDTO commentDTO)
        {
            try
            {
                var comment = new Comment
                {
                    PostId = commentDTO.PostId,
                    Text = commentDTO.Text,
                    AppUserId = commentDTO.AppUserId
                };

                _context.Comments.Add(comment);
                _context.SaveChanges();
            }
            catch (Exception)
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}
