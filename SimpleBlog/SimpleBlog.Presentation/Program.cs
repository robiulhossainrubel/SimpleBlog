using Serilog;
using SimpleBlog.Infrastructure.DI;
using SimpleBlog.Infrastructure.DI.AuthFilter;

namespace SimpleBlog.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                //builder.Logging.AddLog4Net();
                Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day).CreateLogger();
                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
                builder.Host.UseSerilog();
                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddInfrastructureService(builder.Configuration);

                builder.Services.AddAuthorization(option =>
                {
                    option.AddPolicy("CheckBlockUser", policy => policy.Requirements.Add(new CheckBlockUser()));
                });

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
