using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface ILikeDisLikeService
    {
        int Create(LikeDisLike like);
        LikeDisLike Get(int id);
        void Update(LikeDisLike like);
        List<LikeDisLike> GetAll();
    }
}
