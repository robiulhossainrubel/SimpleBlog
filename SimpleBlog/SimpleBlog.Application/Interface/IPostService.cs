using SimpleBlog.Application.DTOs;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IPostService
    {
        void Create(Post post);
        void Update(Post post);
        Post Get(int id);
        List<Post> GetAll();
        Pagination<Post> GetPaginate(int pageIndex, int pageSize);
    }
}
