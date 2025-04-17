using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services
{
    public class ReactionService(BlogDbContext context) : IReactionService
    {
        public void Create(Reaction reaction)
        {
            context.Reactions.Add(reaction);
            context.SaveChanges();
        }

        public void Delete(int postId, int userId)
        {
            var react = GetByPostIdAndUserId(postId, userId);
            context.Reactions.Remove(react);
            context.SaveChanges();
        }

        public Reaction Get(int id)
        {
            return context.Reactions.FirstOrDefault(x => x.Id == id);
        }

        public List<Reaction> GetAll()
        {
            var reactionslist = context.Reactions.ToList();

            return reactionslist;
        }

        public Reaction GetByPostIdAndUserId(int postId, int userId)
        {
            return context.Reactions.FirstOrDefault(x => x.PostId == postId && x.AppUserId == userId);
        }

        public void Update(Reaction reaction)
        {
            context.Reactions.Update(reaction);
            context.SaveChanges();
        }
    }
}
