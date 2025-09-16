using Confluent.Kafka;
using System.Text.Json;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class KafkaActivityProducer : IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;
        private readonly PersistentActivityQueue _persistentQueue;
        private readonly SemaphoreSlim _semaphore;

        public KafkaActivityProducer(PersistentActivityQueue persistentQueue)
        {
            _persistentQueue = persistentQueue;
            
            // Limit concurrent Kafka operations to prevent resource exhaustion
            _semaphore = new SemaphoreSlim(10, 10);
            
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092", // Single broker setup
                ClientId = "simpleblog-activity-producer",
                Acks = Acks.All, // Required when EnableIdempotence is true
                MessageTimeoutMs = 3000, // Shorter timeout for better responsiveness
                SocketTimeoutMs = 2000,
                RetryBackoffMs = 100,
                MessageSendMaxRetries = 3,
                EnableIdempotence = true, // Ensure message ordering and prevent duplicates
                QueueBufferingMaxMessages = 10000,
                QueueBufferingMaxKbytes = 10240, // 10MB buffer
                LingerMs = 5 // Small delay to batch messages
            };
            
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topic = "user-activity-logs";
        }

        public async Task SendActivityAsync(UserActivityLog activity)
        {
            // Non-blocking send - fire and forget
            try
            {
                await _semaphore.WaitAsync(TimeSpan.FromMilliseconds(100)); // Don't wait too long
                
                var json = JsonSerializer.Serialize(activity);
                var message = new Message<Null, string> { Value = json };
                
                // Use Produce (non-blocking) instead of ProduceAsync for better performance
                _producer.Produce(_topic, message, DeliveryHandler);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preparing to send activity to Kafka: {ex.Message}");
                // If we can't even prepare to send, add to persistent queue as backup
                EnqueueToPersistentStorage(activity);
            }
            finally
            {
                // Always release the semaphore
                try
                {
                    _semaphore.Release();
                }
                catch
                {
                    // Ignore semaphore release errors
                }
            }
        }

        private void DeliveryHandler(DeliveryReport<Null, string> report)
        {
            // This is called when the message delivery is confirmed or failed
            if (report.Error.IsError)
            {
                Console.WriteLine($"Failed to deliver message to Kafka: {report.Error.Reason}");
                // We don't have the activity object here, so we can't easily fallback
                // In a production system, you might want to implement a more robust fallback
            }
            else
            {
                Console.WriteLine($"Delivered message to Kafka: {report.TopicPartitionOffset}");
            }
        }

        public async Task SendBatchActivitiesAsync(List<UserActivityLog> activities)
        {
            if (activities == null || activities.Count == 0)
                return;

            try
            {
                // For batches, we can still use the semaphore but with a higher limit
                await _semaphore.WaitAsync(TimeSpan.FromMilliseconds(500));
                
                var tasks = new List<Task>();
                
                foreach (var activity in activities)
                {
                    var json = JsonSerializer.Serialize(activity);
                    var message = new Message<Null, string> { Value = json };
                    
                    // Use Produce with callback for better performance
                    _producer.Produce(_topic, message, DeliveryHandler);
                }
                
                Console.WriteLine($"Queued {activities.Count} activities to Kafka");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preparing batch send to Kafka: {ex.Message}");
                // If we can't even prepare to send, add all to persistent queue as backup
                foreach (var activity in activities)
                {
                    EnqueueToPersistentStorage(activity);
                }
            }
            finally
            {
                try
                {
                    _semaphore.Release();
                }
                catch
                {
                    // Ignore semaphore release errors
                }
            }
        }

        private void EnqueueToPersistentStorage(UserActivityLog activity)
        {
            try
            {
                // This should also be fire-and-forget to avoid blocking
                _persistentQueue.Enqueue(activity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to enqueue activity to persistent storage: {ex.Message}");
            }
        }

        public void Dispose()
        {
            try
            {
                // Flush any remaining messages with a short timeout
                _producer?.Flush(TimeSpan.FromSeconds(2));
            }
            catch
            {
                // Ignore flush errors during disposal
            }
            
            _producer?.Dispose();
            _semaphore?.Dispose();
        }
    }
}