using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;
using SimpleBlog.Infrastructure.Services.ActivityLogging;
using SimpleBlog.Infrastructure.Services.Business;

namespace SimpleBlog.Infrastructure.DI
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlogDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<AppUser, AppUserRole>(options => options.SignIn.RequireConfirmedAccount = false).AddDefaultTokenProviders()
                .AddEntityFrameworkStores<BlogDbContext>();

            services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Auth/Auth/SignIn";
                option.AccessDeniedPath = "/Auth/Auth/AccessDenied";
                option.ExpireTimeSpan = TimeSpan.FromDays(1);
            });

            services.AddScoped<ISeedData, SeedData>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IReactionService, ReactionService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddSingleton<IUserActivityService, UserActivityService>();
            services.AddSingleton<PersistentActivityQueue>();
            services.AddSingleton<KafkaActivityProducer>();
            services.AddSingleton<KafkaActivityConsumer>();
            services.AddHostedService<PersistentActivityQueue>(provider => provider.GetService<PersistentActivityQueue>());
            services.AddHostedService<PersistentQueueProcessor>();

            return services;
        }

        public static IApplicationBuilder UseDataSeed(this IApplicationBuilder application)
        {
            using (var scope = application.ApplicationServices.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ISeedData>();
                service.Initialize();
            }

            return application;
        }

        public static IApplicationBuilder UseActivityLogging(this IApplicationBuilder application)
        {
            // Start the Kafka consumer
            var kafkaConsumer = application.ApplicationServices.GetService<KafkaActivityConsumer>();
            if (kafkaConsumer != null)
            {
                kafkaConsumer.StartConsuming();
            }

            return application;
        }
    }
}