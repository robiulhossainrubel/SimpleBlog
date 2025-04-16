using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services
{
    public class LikeDisLikeService(BlogDbContext context) : ILikeDisLikeService
    {
        public int Create(LikeDisLike like)
        {
            context.LikeDisLikes.Add(like);
            context.SaveChanges();

            return like.Id;
        }

        public LikeDisLike Get(int id)
        {
            return context.LikeDisLikes.FirstOrDefault(x => x.Id == id);
        }

        public List<LikeDisLike> GetAll()
        {
            var likeDislikes = context.LikeDisLikes.ToList();

            return likeDislikes;
        }

        public void Update(LikeDisLike like)
        {
            context.Update(like);
            context.SaveChanges();
        }
    }
}
