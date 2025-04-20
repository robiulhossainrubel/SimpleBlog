using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services
{
    public class ReactionService(BlogDbContext context) : IReactionService
    {
        public void Create(Reaction reaction)
        {
            try
            {
                context.Reactions.Add(reaction);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Delete(int postId, int userId)
        {
            try
            {
                var react = GetByPostIdAndUserId(postId, userId);

                context.Reactions.Remove(react);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
            try
            {
                context.Reactions.Update(reaction);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void React(int postId, int reactId, int userId)
        {
            try
            {
                var reaction = GetByPostIdAndUserId(postId, userId);

                if (reaction == null)
                {
                    var react = new Reaction
                    {
                        ReactType = (ReactionType)reactId,
                        PostId = postId,
                        AppUserId = userId
                    };

                    Create(react);
                }
                else
                {
                    if (reaction.ReactType == (ReactionType)reactId)
                    {
                        Delete(postId, userId);
                    }
                    else
                    {
                        reaction.ReactType = (ReactionType)reactId;

                        Update(reaction);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
