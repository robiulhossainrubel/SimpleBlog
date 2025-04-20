using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IReactionService
    {
        void Create(Reaction reaction);
        void Update(Reaction reaction);
        Reaction Get(int id);
        Reaction GetByPostIdAndUserId(int postId, int userId);
        void Delete(int postId, int userId);
        List<Reaction> GetAll();
        void React(int postId, int reactId, int userId);
    }
}
