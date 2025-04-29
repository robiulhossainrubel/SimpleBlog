using System.Linq.Expressions;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IPostService
    {
        Task Create(PostDTO postDTO);
        Task Update(Post post);
        Post Get(int id);
        List<Post> GetAll(Expression<Func<Post, bool>>? expression = null);
        List<Post> TopPosts();
        Pagination<Post> GetPaginate(int pageIndex, int pageSize);
    }
}
