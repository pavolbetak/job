using App.Database;
using App.HostedServices;
using App.RabbitMq;
using App.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .CreateLogger();

            LoadConstants();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMemoryCache();

            services.AddTransient<StorageService>();
            services.AddTransient<MessageBroker>();

            services.AddHostedService<LogCalculationDataHostedService>();

            services.AddSingleton(sp =>
            {
                // Read from configuration
                var factory = new ConnectionFactory { HostName = "localhost", UserName = "admin", Password="admin" };
                var connection = factory.CreateConnection();

                return connection;
            });

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(AppConst.DatabaseConnectionString));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapControllers();
            });
        }

        private void LoadConstants()
        {
            AppConst.DatabaseConnectionString = Configuration["ConnectionStrings:Default"];
        }
    }
}
