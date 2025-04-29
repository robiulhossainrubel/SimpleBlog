using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Repository
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(BlogDbContext context) : base(context)
        {
        }
    }
}
