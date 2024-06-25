using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

public class MessagePublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    
    public MessagePublisher()
    {
        var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Publish<T>(T message, string queueName)
    {
        var messageBody = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(messageBody);

        var queueDlq = queueName + "Dlq";

        var args = new Dictionary<string, object>
        {
            {"x-dead-letter-exchange", ""},
            {"x-dead-letter-routing-key", queueDlq},
            {"x-message-ttl", 60000},  
            {"x-max-length", 10000}, 
            {"x-max-delivery-count", 5}
        };

        _channel.QueueDeclare(queue: queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: args);

         _channel.QueueDeclare(queue: queueDlq,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.BasicPublish(exchange: "",
                              routingKey: queueName,
                              basicProperties: null,
                              body: body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
