using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IReactionService
    {
        List<Reaction> GetAll();
        Task React(int postId, int reactId, int userId);
    }
}
