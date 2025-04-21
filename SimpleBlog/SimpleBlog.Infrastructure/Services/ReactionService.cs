using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Infrastructure.Services
{
    public class ReactionService : IReactionService
    {
        private readonly BlogDbContext _context;
        public ReactionService(BlogDbContext context)
        {
            _context = context;
        }
        public void Create(Reaction reaction)
        {
            try
            {
                _context.Reactions.Add(reaction);
                _context.SaveChanges();
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

                _context.Reactions.Remove(react);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Reaction Get(int id)
        {
            var reaction = _context.Reactions.FirstOrDefault(x => x.Id == id);

            return reaction;
        }

        public List<Reaction> GetAll()
        {
            var reactionslist = _context.Reactions.ToList();

            return reactionslist;
        }

        public Reaction GetByPostIdAndUserId(int postId, int userId)
        {
            var reaction = _context.Reactions.FirstOrDefault(x => x.PostId == postId && x.AppUserId == userId);

            return reaction;
        }

        public void Update(Reaction reaction)
        {
            try
            {
                _context.Reactions.Update(reaction);
                _context.SaveChanges();
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
