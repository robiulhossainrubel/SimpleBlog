using Confluent.Kafka;
using System.Text.Json;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Application.Interface;

namespace SimpleBlog.Infrastructure.Services.ActivityLogging
{
    public class KafkaActivityConsumer : IDisposable
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly IUserActivityService _activityService;
        private readonly string _topic;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task? _consumingTask;
        private readonly SemaphoreSlim _processingSemaphore;

        public KafkaActivityConsumer(IUserActivityService activityService)
        {
            _activityService = activityService;
            
            // Limit concurrent processing to prevent overwhelming ClickHouse
            _processingSemaphore = new SemaphoreSlim(10, 10);
            
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092", // Single broker setup
                GroupId = "simpleblog-activity-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                SessionTimeoutMs = 45000,
                HeartbeatIntervalMs = 3000,
                EnablePartitionEof = false,
                MaxPollIntervalMs = 300000, // 5 minutes
                MessageMaxBytes = 1048576, // 1MB
                QueuedMinMessages = 1000,
                QueuedMaxMessagesKbytes = 10240 // 10MB
            };
            
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
            _topic = "user-activity-logs";
        }

        public void StartConsuming()
        {
            _consumingTask = Task.Run(ConsumeMessagesAsync, _cancellationTokenSource.Token);
        }

        private async Task ConsumeMessagesAsync()
        {
            _consumer.Subscribe(_topic);
            
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Use shorter timeout for more responsive shutdown
                        var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(500));
                        
                        if (consumeResult != null && !_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            // Process messages with concurrency control
                            if (await _processingSemaphore.WaitAsync(TimeSpan.FromMilliseconds(100), _cancellationTokenSource.Token))
                            {
                                // Fire-and-forget processing to avoid blocking the consumer loop
                                _ = Task.Run(async () =>
                                {
                                    try
                                    {
                                        await ProcessMessageAsync(consumeResult);
                                    }
                                    finally
                                    {
                                        try
                                        {
                                            _processingSemaphore.Release();
                                        }
                                        catch
                                        {
                                            // Ignore semaphore release errors
                                        }
                                    }
                                });
                            }
                            else
                            {
                                // If we can't process the message right now, log it
                                Console.WriteLine("Skipping message processing due to high load");
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        Console.WriteLine($"Error consuming message: {ex.Error.Reason}");
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected during shutdown
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error consuming message: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            finally
            {
                _consumer.Close();
            }
        }

        private async Task ProcessMessageAsync(ConsumeResult<Null, string> consumeResult)
        {
            try
            {
                var activityJson = consumeResult.Message.Value;
                var activity = JsonSerializer.Deserialize<UserActivityLog>(activityJson);
                
                if (activity != null)
                {
                    // Store in ClickHouse (this is already non-blocking in the activity service)
                    await _activityService.LogActivityBulkAsync(new List<UserActivityLog> { activity });
                    Console.WriteLine($"Stored activity in ClickHouse: {activity.Controller}/{activity.Action}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                // In a production system, you might want to send failed messages to a dead letter queue
            }
        }

        public async Task StopConsumingAsync()
        {
            _cancellationTokenSource.Cancel();
            
            // Wait for any ongoing processing to complete (with timeout)
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try
            {
                await _processingSemaphore.WaitAsync(cts.Token);
                _processingSemaphore.Release();
            }
            catch (OperationCanceledException)
            {
                // Timeout waiting for processing to complete
                Console.WriteLine("Timed out waiting for message processing to complete");
            }
            
            if (_consumingTask != null)
            {
                try
                {
                    // Give the consumer loop time to exit gracefully
                    await Task.WhenAny(_consumingTask, Task.Delay(5000));
                }
                catch (OperationCanceledException)
                {
                    // Expected
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            
            try
            {
                _consumer?.Close();
            }
            catch
            {
                // Ignore errors during consumer close
            }
            
            try
            {
                _consumer?.Dispose();
            }
            catch
            {
                // Ignore errors during consumer disposal
            }
            
            try
            {
                _processingSemaphore?.Dispose();
            }
            catch
            {
                // Ignore errors during semaphore disposal
            }
        }
    }
}