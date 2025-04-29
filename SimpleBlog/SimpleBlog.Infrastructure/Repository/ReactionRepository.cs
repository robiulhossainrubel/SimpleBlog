using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Repository
{
    public class ReactionRepository : GenericRepository<Reaction>, IReactionRepository
    {
        public ReactionRepository(BlogDbContext context) : base(context)
        {
        }
    }
}
