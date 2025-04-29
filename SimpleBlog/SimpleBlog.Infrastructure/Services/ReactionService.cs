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

        public async Task Create(Reaction reaction)
        {
            try
            {
                await _repository.Create(reaction);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Delete(int postId, int userId)
        {
            try
            {
                var react = GetByPostIdAndUserId(postId, userId);

                await _repository.Delete(react);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Reaction Get(int id)
        {
            try
            {
                var reaction = _repository.GetById(id);

                return reaction;
            }
            catch (Exception)
            {
                throw;
            }
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

        public Reaction GetByPostIdAndUserId(int postId, int userId)
        {
            try
            {
                var reaction = _repository.Get(x => x.PostId == postId && x.AppUserId == userId);

                return reaction;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Update(Reaction reaction)
        {
            try
            {
                await _repository.Update(reaction);
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
                var reaction = GetByPostIdAndUserId(postId, userId);

                if (reaction == null)
                {
                    var react = new Reaction
                    {
                        ReactType = (ReactionType)reactId,
                        PostId = postId,
                        AppUserId = userId
                    };

                    await Create(react);
                }
                else
                {
                    if (reaction.ReactType == (ReactionType)reactId)
                    {
                        await Delete(postId, userId);
                    }
                    else
                    {
                        reaction.ReactType = (ReactionType)reactId;

                        await Update(reaction);
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
