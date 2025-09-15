using System.Collections.Concurrent;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class UserActivityQueue
    {
        private readonly ConcurrentQueue<UserActivityLog> _queue = new();
        private readonly IUserActivityService _service;
        private readonly CancellationTokenSource _cts = new();

        public UserActivityQueue(IUserActivityService service)
        {
            _service = service;
            Task.Run(ProcessQueueAsync);
        }

        public void Enqueue(UserActivityLog activity)
        {
            _queue.Enqueue(activity);
        }

        private async Task ProcessQueueAsync()
        {
            var batch = new List<UserActivityLog>();
            while (!_cts.Token.IsCancellationRequested)
            {
                while (_queue.TryDequeue(out var activity))
                {
                    batch.Add(activity);
                    if (batch.Count >= 500) break;
                }

                if (batch.Count > 0)
                {
                    try
                    {
                        await _service.LogActivityBulkAsync(batch);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bulk logging failed: {ex.Message}");
                    }
                    batch.Clear();
                }

                await Task.Delay(50);
            }
        }

        public void Stop() => _cts.Cancel();
    }
}
