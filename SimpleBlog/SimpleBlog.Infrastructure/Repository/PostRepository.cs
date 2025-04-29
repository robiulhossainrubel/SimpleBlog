using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Repository
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(BlogDbContext context) : base(context)
        {
        }
    }
}
