using Demo2.Core;
using Microsoft.EntityFrameworkCore;
using Quartz.AspNetCore;
using Quartz;
using Serilog;

namespace Demo2.Service
{
    public class Setup
    {
        public static async Task<WebApplication> ComposeApplication()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            builder.Services.AddDbContext<Context>();

            builder.Services.AddScoped<Context, Context>();
            builder.Services.AddScoped<Demonstrator, Demonstrator>();

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();

            builder.Services.AddQuartz(q =>
            {
                var key = new JobKey("1");

                q.AddJob<Runner>(opts => opts.WithIdentity(key));
                q.AddTrigger(opts => opts.ForJob(key)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(60).
                        RepeatForever()));
            });

            builder.Services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseSwagger();
            app.UseSwaggerUI();

            await app.Services.CreateScope().ServiceProvider.GetRequiredService<Context>().Database.MigrateAsync();

            return app;
        }
    }
}
