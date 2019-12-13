using System;
using DatingApp.API.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DatingApp.API
{
    public class Program
    {
        private static object Host;

        public static void Main (string[] args)
        {
            var host = CreateHostBuilder (args).Build ();
            using (var scope = host.Services.CreateScope ())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DataContext> ();
                    context.Database.Migrate ();
                    Seed.SeedUsers (context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>> ();
                    logger.LogError (ex, "An error occurred during migration");
                }
            }

            host.Run ();
        }

        public static IWebHostBuilder CreateHostBuilder (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ();
    }
    // Host.CreateDefaultBuilder (args).ConfigureWebHostDefaults (webBuilder =>
    // {
    //     webBuilder.UseStartup<Startup> ();
    // });
}