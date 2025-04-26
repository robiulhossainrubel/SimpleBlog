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
            catch
            {
                throw;
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
            catch
            {
                throw;
            }
        }

        public Reaction Get(int id)
        {
            try
            {
                var reaction = _context.Reactions.FirstOrDefault(x => x.Id == id);

                return reaction;
            }
            catch
            {
                throw;
            }
        }

        public List<Reaction> GetAll()
        {
            try
            {
                var reactionslist = _context.Reactions.ToList();

                return reactionslist;
            }
            catch
            {
                throw;
            }
        }

        public Reaction GetByPostIdAndUserId(int postId, int userId)
        {
            try
            {
                var reaction = _context.Reactions.FirstOrDefault(x => x.PostId == postId && x.AppUserId == userId);

                return reaction;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Reaction reaction)
        {
            try
            {
                _context.Reactions.Update(reaction);
                _context.SaveChanges();
            }
            catch
            {
                throw;
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
            catch
            {
                throw;
            }
        }
    }
}
