using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface ICommentService
    {
        void Create(Comment comment);
        List<Comment> GetAll();
    }
}
