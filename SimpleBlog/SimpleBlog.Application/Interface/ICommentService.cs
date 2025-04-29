using SimpleBlog.Application.DTOs;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface ICommentService
    {
        Task Create(CommentDTO commentDTO);
        List<Comment> GetAll();
    }
}
