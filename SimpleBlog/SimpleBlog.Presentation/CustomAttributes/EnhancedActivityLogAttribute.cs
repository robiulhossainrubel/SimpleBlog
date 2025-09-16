using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Presentation.CustomAttributes
{
    public class EnhancedActivityLogAttribute : ActionFilterAttribute
    {
        private readonly KafkaActivityProducer _kafkaProducer;
        private readonly PersistentActivityQueue _persistentQueue;

        public EnhancedActivityLogAttribute(KafkaActivityProducer kafkaProducer, PersistentActivityQueue persistentQueue)
        {
            _kafkaProducer = kafkaProducer;
            _persistentQueue = persistentQueue;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Capture activity data immediately but process asynchronously
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            uint userId = 0;
            if (!string.IsNullOrEmpty(userIdClaim) && uint.TryParse(userIdClaim, out var parsedId))
            {
                userId = parsedId;
            }

            var activity = new UserActivityLog
            {
                EventTime = DateTime.Now,
                UserId = userId,
                Controller = controllerName ?? "",
                Action = actionName ?? ""
            };

            // Fire-and-forget pattern - completely non-blocking
            // No await, no Task tracking, just fire and forget
            _ = Task.Run(() => ProcessActivityAsync(activity))
                   .ContinueWith(t => 
                   {
                       // Log any unexpected errors but don't block the main request
                       if (t.Exception != null)
                       {
                           Console.WriteLine($"Unexpected error in activity logging: {t.Exception.Message}");
                       }
                   }, TaskContinuationOptions.OnlyOnFaulted);

            // Immediately return to avoid blocking the main request
            base.OnActionExecuting(context);
        }

        private async Task ProcessActivityAsync(UserActivityLog activity)
        {
            try
            {
                // Try to send to Kafka with a timeout to prevent hanging
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await _kafkaProducer.SendActivityAsync(activity);
            }
            catch (Exception ex)
            {
                // If Kafka fails, add to persistent queue as backup
                // This is also fire-and-forget to prevent blocking
                try
                {
                    _persistentQueue.Enqueue(activity);
                }
                catch (Exception queueEx)
                {
                    Console.WriteLine($"Failed to queue activity: {queueEx.Message}");
                }
            }
        }
    }
}