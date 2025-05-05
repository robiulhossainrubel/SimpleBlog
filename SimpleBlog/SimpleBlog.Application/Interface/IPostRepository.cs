using System.Linq.Expressions;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Post GetPost(int id);
        List<Post> GetAllPosts(Expression<Func<Post, bool>>? expression = null);
        Pagination<Post> GetPaginate(int pageIndex, int pageSize);
        List<Post> TopPosts();
    }
}
