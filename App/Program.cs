using App.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbBuilder = new DbContextOptionsBuilder<AppDbContext>();
                dbBuilder.UseNpgsql(AppConst.DatabaseConnectionString);
                var options = dbBuilder.Options;

                var db = new AppDbContext(options);
                try
                {
                    db.Database.ExecuteSql($"CREATE DATABASE jobDb");
                }
                catch (PostgresException postEx) when (postEx.Message.Contains("exist"))
                {
                    Log.Information("INFO : Database already exist");
                }
                db.Database.Migrate();

            }

            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
