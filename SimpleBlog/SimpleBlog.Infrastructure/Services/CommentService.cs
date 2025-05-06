using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;

        public CommentService(ICommentRepository repository)
        {
            _repository = repository;
        }

        #region Operational
        public async Task Create(CommentDTO commentDto)
        {
            try
            {
                var comment = new Comment
                {
                    PostId = commentDto.PostId,
                    Text = commentDto.Text,
                    AppUserId = commentDto.AppUserId
                };

                await _repository.Create(comment);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region List Loading
        public List<Comment> GetAll()
        {
            try
            {
                var comments = _repository.GetAll(includeProperties: "AppUser").ToList();

                return comments;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
