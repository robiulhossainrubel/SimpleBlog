using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _repository;

        public ReactionService(IReactionRepository repository)
        {
            _repository = repository;
        }

        public List<Reaction> GetAll()
        {
            try
            {
                var reactionslist = _repository.GetAll().ToList();

                return reactionslist;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task React(int postId, int reactId, int userId)
        {
            try
            {
                var reaction = _repository.Get(x => x.PostId == postId && x.AppUserId == userId);

                if (reaction == null)
                {
                    var react = new Reaction
                    {
                        ReactType = (ReactionType)reactId,
                        PostId = postId,
                        AppUserId = userId
                    };

                    await _repository.Create(react);
                }
                else
                {
                    if (reaction.ReactType == (ReactionType)reactId)
                    {
                        await _repository.Delete(reaction);
                    }
                    else
                    {
                        reaction.ReactType = (ReactionType)reactId;

                        await _repository.Update(reaction);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
