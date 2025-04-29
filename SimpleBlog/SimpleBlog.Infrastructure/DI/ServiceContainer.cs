using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;
using SimpleBlog.Infrastructure.DI.AuthFilter;
using SimpleBlog.Infrastructure.Repository;
using SimpleBlog.Infrastructure.Services;

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

            //services.AddAuthorization(option =>
            //{
            //    option.AddPolicy("CheckBlockUser", policy => policy.Requirements.Add(new CheckBlockUser()));
            //});

            services.AddScoped<ISeedData, SeedData>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IReactionRepository, ReactionRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IReactionService, ReactionService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IAuthorizationHandler, CheckBlockUserHandler>();

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
    }
}
