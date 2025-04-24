using Microsoft.AspNetCore.Authorization;
using SimpleBlog.Infrastructure.DI;
using SimpleBlog.Presentation.RD;

namespace SimpleBlog.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddInfrastructureService(builder.Configuration);
            builder.Services.AddAuthorization(option =>
            {
                option.AddPolicy("CheckUser", policy => policy.Requirements.Add(new CheckUser()));
            });
            builder.Services.AddScoped<IAuthorizationHandler, CheckUserHandler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseDataSeed();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
