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
                .WriteTo.Console()
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
                var factory = new ConnectionFactory { HostName = AppConst.RabbitMqHostName, UserName = "admin", Password = "admin" };
                var connection = factory.CreateConnection();

                return connection;
            });

            services.AddSwaggerGen();

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

            app.UseSwagger();
            app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
        }

        private void LoadConstants()
        {
            AppConst.DatabaseConnectionString = Configuration["ConnectionStrings:Default"];
            AppConst.RabbitMqHostName = Configuration["RabbitMq:HostName"];
        }
    }
}
