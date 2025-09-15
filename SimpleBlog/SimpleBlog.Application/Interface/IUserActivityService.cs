using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IUserActivityService
    {
        void EnsureTables();
        void LogActivity(UserActivityLog activity);
    }
}
