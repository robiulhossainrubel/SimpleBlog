using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.CustomAttributes;

public class ActivityLogAttribute : ActionFilterAttribute
{
    private readonly IUserActivityService _activityService;

    public ActivityLogAttribute(IUserActivityService activityService)
    {
        _activityService = activityService;
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

        _ = Task.Run(() =>
        {
            try
            {
                _activityService.LogActivity(activity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        });
        base.OnActionExecuting(context);
    }
}
