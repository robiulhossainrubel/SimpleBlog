using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IUserActivityService
    {
        void EnsureTables();
        Task LogActivityBulkAsync(List<UserActivityLog> activities);
    }
}
