using App.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace App.RabbitMq
{
    public class MessageBroker
    {
        private readonly IConnection _connection;

        private string _exchange => "calculation";

        public MessageBroker(IConnection connection)
        {
            _connection = connection;
        }

        public void PublishCalculationData(CalculatedData calculationData)
        {
            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Fanout);

            string message = JsonSerializer.Serialize(calculationData);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: _exchange,
                                 routingKey: string.Empty,
                                 basicProperties: null,
            body: body);
        }
    }
}
