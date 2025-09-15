using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Presentation.CustomAttributes;

public class ActivityLogAttribute : ActionFilterAttribute
{
    private readonly UserActivityQueue _activityQueue;

    public ActivityLogAttribute(UserActivityQueue activityQueue)
    {
        _activityQueue = activityQueue;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var referrerUrl = context.HttpContext.Request.Headers["Referer"].ToString();

        uint userId = 0;
        if (!string.IsNullOrEmpty(userIdClaim) && uint.TryParse(userIdClaim, out var parsedId))
        {
            userId = parsedId;
        }

        var activity = new UserActivityLog
        {
            EventTime = DateTime.Now,
            UserId = userId,
            Controller = $"{controllerName}",
            Action = $"{actionName}"
        };

        _activityQueue.Enqueue(activity);

        base.OnActionExecuting(context);
    }
}
