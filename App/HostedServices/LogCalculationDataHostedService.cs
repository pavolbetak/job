using RabbitMQ.Client;
using App.RabbitMq;
using Serilog;

namespace App.HostedServices
{
    public class LogCalculationDataHostedService : BackgroundService
    {
        private readonly IConnection _connection;

        private readonly MessageBroker _messageBroker;

        public LogCalculationDataHostedService(IConnection connection, MessageBroker messageBroker)
        {
            _connection = connection;
            _messageBroker = messageBroker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var channel = _connection.CreateModel();

                    _messageBroker.RegisterToReceiveCalculationData(channel, (message) =>
                    {
                        Log.Information("{@Message}", message);
                    });

                    Log.Information("Listening for calculation data");

                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    var retryInMs = 5000;

                    Log.Error(ex, "Failed to subscribe for calculation updates. Retrying in {RetryInMs}ms", retryInMs);

                    await Task.Delay(retryInMs);
                }
            }
        }
    }
}
