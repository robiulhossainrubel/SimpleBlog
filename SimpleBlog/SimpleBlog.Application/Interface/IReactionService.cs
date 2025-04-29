using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IReactionService
    {
        Task Create(Reaction reaction);
        Task Update(Reaction reaction);
        Reaction Get(int id);
        Reaction GetByPostIdAndUserId(int postId, int userId);
        Task Delete(int postId, int userId);
        List<Reaction> GetAll();
        Task React(int postId, int reactId, int userId);
    }
}
