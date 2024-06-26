using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using App.RabbitMq;
using Serilog;
using App.Services;

namespace App.HostedServices
{
    public class LogCalculationDataHostedService : BackgroundService
    {
        private readonly IConnection _connection;

        public LogCalculationDataHostedService(IConnection connection, MessageBroker messageBroker)
        {
            _connection = connection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var channel = _connection.CreateModel();
                    channel.ExchangeDeclare(exchange: "calculation", type: ExchangeType.Fanout);

                    // declare a server-named queue
                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: queueName,
                                      exchange: "calculation",
                                      routingKey: string.Empty);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += LogCalculationData;
                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    var repeatTime = 5000;

                    Log.Warning($"Cannot subsribe to receive calculation data. Try again after {repeatTime}ms.");

                    await Task.Delay(repeatTime);
                }
            }
        }

        private void LogCalculationData(object? model, BasicDeliverEventArgs ea)
        {
            try
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                // should be logged
            }
        }
    }
}
