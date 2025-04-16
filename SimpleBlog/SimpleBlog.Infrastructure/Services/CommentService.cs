using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services
{
    public class CommentService(BlogDbContext context) : ICommentService
    {
        public void Create(Comment comment)
        {
            context.Comments.Add(comment);
            context.SaveChanges();
        }

        public List<Comment> GetAll()
        {
            var comments = context.Comments.Include(c => c.AppUser).ToList();

            return comments;
        }
    }
}
