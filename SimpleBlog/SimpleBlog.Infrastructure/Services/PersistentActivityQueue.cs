using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class PersistentActivityQueue : IHostedService
    {
        private readonly ConcurrentQueue<UserActivityLog> _memoryQueue = new();
        private readonly string _persistenceFilePath;
        private readonly Timer _persistenceTimer;
        private readonly SemaphoreSlim _persistenceSemaphore = new(1, 1);
        private DateTime _lastPersistTime = DateTime.MinValue;
        private readonly TimeSpan _minPersistenceInterval = TimeSpan.FromSeconds(2);
        private bool _isDirty = false;

        public PersistentActivityQueue()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDirectory = Path.Combine(appDataPath, "SimpleBlog", "ActivityLogs");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }
            
            _persistenceFilePath = Path.Combine(appDirectory, "activity_queue.json");
            
            // Timer for periodic persistence (but not too frequent)
            _persistenceTimer = new Timer(async _ => await PersistQueueToFileAsync(), null, 
                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        public void Enqueue(UserActivityLog activity)
        {
            _memoryQueue.Enqueue(activity);
            _isDirty = true;
            
            // Only persist immediately if we have a lot of items or enough time has passed
            if (_memoryQueue.Count > 100 || DateTime.Now - _lastPersistTime > _minPersistenceInterval)
            {
                // Fire-and-forget persistence to avoid blocking
                _ = Task.Run(async () => await PersistQueueToFileAsync());
            }
        }

        private async Task PersistQueueToFileAsync()
        {
            // Don't persist too frequently to avoid I/O overhead
            if (!_isDirty || DateTime.Now - _lastPersistTime < _minPersistenceInterval)
            {
                return;
            }

            // Use semaphore to prevent concurrent persistence operations
            if (!await _persistenceSemaphore.WaitAsync(TimeSpan.FromMilliseconds(100)))
            {
                return; // Another persistence operation is already running
            }

            try
            {
                // Only persist if there are actually items to persist
                if (_memoryQueue.IsEmpty)
                {
                    _isDirty = false;
                    return;
                }
                
                // Convert queue to list for serialization (non-blocking)
                var activities = new List<UserActivityLog>();
                var tempQueue = new ConcurrentQueue<UserActivityLog>(_memoryQueue);
                
                while (tempQueue.TryDequeue(out var activity))
                {
                    activities.Add(activity);
                }
                
                var json = JsonSerializer.Serialize(activities, new JsonSerializerOptions { WriteIndented = false });
                await File.WriteAllTextAsync(_persistenceFilePath, json);
                
                _lastPersistTime = DateTime.Now;
                _isDirty = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to persist activity queue: {ex.Message}");
            }
            finally
            {
                _persistenceSemaphore.Release();
            }
        }

        private void LoadQueueFromFile()
        {
            try
            {
                if (File.Exists(_persistenceFilePath))
                {
                    var json = File.ReadAllText(_persistenceFilePath);
                    var activities = JsonSerializer.Deserialize<List<UserActivityLog>>(json) ?? new List<UserActivityLog>();
                    
                    // Clear current queue and repopulate
                    while (_memoryQueue.TryDequeue(out _)) { }
                    
                    foreach (var activity in activities)
                    {
                        _memoryQueue.Enqueue(activity);
                    }
                    
                    Console.WriteLine($"Loaded {_memoryQueue.Count} activities from persistent storage");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load activity queue: {ex.Message}");
            }
        }

        public bool TryDequeue(out UserActivityLog activity)
        {
            var result = _memoryQueue.TryDequeue(out activity);
            if (result)
            {
                _isDirty = true;
                // Schedule persistence but don't block
                _ = Task.Run(async () => await PersistQueueToFileAsync());
            }
            return result;
        }

        public int Count => _memoryQueue.Count;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            LoadQueueFromFile();
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Final persistence before shutdown
            await PersistQueueToFileAsync();
            
            _persistenceTimer?.Dispose();
            _persistenceSemaphore?.Dispose();
        }
    }
}