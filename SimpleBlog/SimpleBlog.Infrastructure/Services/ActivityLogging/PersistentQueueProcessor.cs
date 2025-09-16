using Microsoft.Extensions.Hosting;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services.ActivityLogging;

namespace SimpleBlog.Infrastructure.Services.ActivityLogging
{
    public class PersistentQueueProcessor : BackgroundService
    {
        private readonly PersistentActivityQueue _persistentQueue;
        private readonly KafkaActivityProducer _kafkaProducer;
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

        public PersistentQueueProcessor(PersistentActivityQueue persistentQueue, KafkaActivityProducer kafkaProducer)
        {
            _persistentQueue = persistentQueue;
            _kafkaProducer = kafkaProducer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Start with a quick check, then back off to longer intervals
            var delay = TimeSpan.FromSeconds(5);
            var maxDelay = TimeSpan.FromMinutes(2);
            var errorCount = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Only process if there are items in the queue
                    if (_persistentQueue.Count > 0)
                    {
                        // Prevent multiple concurrent processing attempts
                        if (await _processingSemaphore.WaitAsync(TimeSpan.FromMilliseconds(100), stoppingToken))
                        {
                            try
                            {
                                await ProcessQueueBatchAsync(stoppingToken);
                                // Reset error count and delay on success
                                errorCount = 0;
                                delay = TimeSpan.FromSeconds(5);
                            }
                            finally
                            {
                                _processingSemaphore.Release();
                            }
                        }
                    }

                    // Wait before checking again
                    await Task.Delay(delay, stoppingToken);

                    // Gradually increase delay if queue is empty
                    if (_persistentQueue.Count == 0 && delay < maxDelay)
                    {
                        delay = TimeSpan.FromTicks(Math.Min(delay.Ticks * 2, maxDelay.Ticks));
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing persistent queue: {ex.Message}");
                    errorCount++;

                    // Increase delay on errors, up to a maximum
                    var errorDelay = TimeSpan.FromSeconds(Math.Min(30 * errorCount, 300)); // Max 5 minutes
                    await Task.Delay(errorDelay, stoppingToken);
                }
            }
        }

        private async Task ProcessQueueBatchAsync(CancellationToken stoppingToken)
        {
            // Process any activities in the persistent queue
            var batch = new List<UserActivityLog>();
            UserActivityLog activity;

            // Collect up to 50 activities to send as a batch (smaller batch for better responsiveness)
            var batchSize = Math.Min(50, _persistentQueue.Count);
            while (batch.Count < batchSize && _persistentQueue.TryDequeue(out activity) && !stoppingToken.IsCancellationRequested)
            {
                batch.Add(activity);
            }

            if (batch.Count > 0)
            {
                await _kafkaProducer.SendBatchActivitiesAsync(batch);
                Console.WriteLine($"Processed {batch.Count} activities from persistent queue");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Wait for any ongoing processing to complete
            await _processingSemaphore.WaitAsync(cancellationToken);
            _processingSemaphore.Release();

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _processingSemaphore?.Dispose();
            base.Dispose();
        }
    }
}