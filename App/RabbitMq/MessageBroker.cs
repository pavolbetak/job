using App.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Text.Json;

namespace App.RabbitMq
{
    public class MessageBroker
    {
        private readonly IConnection _connection;

        private const string Exchange = "calculation";

        public MessageBroker(IConnection connection)
        {
            _connection = connection;
        }

        public void PublishCalculationData(CalculatedData calculationData)
        {
            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Fanout);

            string message = JsonSerializer.Serialize(calculationData);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: Exchange,
                                 routingKey: string.Empty,
                                 basicProperties: null,
            body: body);
        }

        public void RegisterToReceiveCalculationData(IModel channel, Action<CalculatedData> action)
        {
            channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Fanout);

            // declare a server-named queue
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: Exchange,
                              routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, args) =>
            {
                try
                {
                    byte[] body = args.Body.ToArray();
                    var message = JsonSerializer.Deserialize<CalculatedData>(Encoding.UTF8.GetString(body));

                    if (message != null)
                    {
                        try
                        {
                            action?.Invoke(message);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Notify action failed");
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(ex, "Failed to deserialize message");
                }
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
