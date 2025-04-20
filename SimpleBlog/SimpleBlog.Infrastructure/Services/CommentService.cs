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
            try
            {
                context.Comments.Add(comment);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<Comment> GetAll()
        {
            var comments = context.Comments.Include(c => c.AppUser).ToList();

            return comments;
        }
    }
}
