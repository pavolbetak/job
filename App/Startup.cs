using App.Database;
using App.HostedServices;
using App.RabbitMq;
using App.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var databaseConnectionString = Configuration["ConnectionStrings:Default"];
            var rabbitMqHostName = Configuration["RabbitMq:HostName"];
            var rabbitMqUserName = Configuration["RabbitMq:UserName"];
            var rabbitMqPassword = Configuration["RabbitMq:Password"];

            services.AddControllers();

            services.AddMemoryCache();

            services.AddTransient<StorageService>();
            services.AddTransient<MessageBroker>();

            services.AddHostedService<LogCalculationDataHostedService>();

            services.AddSingleton(sp =>
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        // Read from configuration
                        var factory = new ConnectionFactory { HostName = rabbitMqHostName, UserName = rabbitMqUserName, Password = rabbitMqPassword };
                        var connection = factory.CreateConnection();

                        return connection;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Cannot connect to RabbitMq - Retrying");

                        Thread.Sleep(3000);
                    }
                }

                throw new InvalidOperationException("Failed to connect RabbitMq");
            });

            services.AddSwaggerGen();

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(databaseConnectionString));
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
    }
}
