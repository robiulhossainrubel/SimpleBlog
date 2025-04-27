using SimpleBlog.Application.DTOs;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface ICommentService
    {
        void Create(CommentDTO commentDTO);
        List<Comment> GetAll();
    }
}
