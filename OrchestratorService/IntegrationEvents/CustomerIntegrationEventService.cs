using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using OrchestratorService.IntegrationEvents.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Events;


namespace OrchestratorService.IntegrationEvents
{
    public class CustomerIntegrationEventService : ICustomerIntegrationEventService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "CustomerRegisteredExchange";
        private const string ResponseQueueName = "OrchestratorResponseQueue";

        public CustomerIntegrationEventService(IConfiguration configuration)
        {
            var hostname = configuration["RabbitMQ:Hostname"];
            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");
        }

        public async Task<CustomerRegisteredIntegrationEvent> PublishThroughEventBusAsync(CustomerRegisteredIntegrationEvent evt)
        {
            var correlationId = Guid.NewGuid().ToString();
            var responseQueueName = $"ResponseQueue_{correlationId}";
            _channel.QueueDeclare(queue: responseQueueName, durable: true, exclusive: true, autoDelete: true, arguments: null);

            var taskCompletionSource = new TaskCompletionSource<CustomerRegisteredIntegrationEvent>();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var responseEvent = JsonConvert.DeserializeObject<CustomerRegisteredIntegrationEvent>(message);

                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    taskCompletionSource.SetResult(responseEvent);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true); // Requeue the message if CorrelationId does not match
                }
            };

            _channel.BasicConsume(queue: responseQueueName, autoAck: false, consumer: consumer);

            try
            {
                evt.CustomerId = Guid.NewGuid();
                string message = JsonConvert.SerializeObject(evt);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = _channel.CreateBasicProperties();
                properties.ReplyTo = responseQueueName;
                properties.CorrelationId = correlationId;

                _channel.BasicPublish(exchange: "",
                                     routingKey: "CustomerRegisteredQueue",
                                     basicProperties: properties,
                                     body: body);
                Console.WriteLine(" [x] Sent {0} with CorrelationId {1}", message, correlationId);

                var responseEvent = await taskCompletionSource.Task;

                if (responseEvent != null && responseEvent.CustomerId != Guid.Empty)
                {
                    Console.WriteLine("Customer creation successful.");
                    return responseEvent;
                }
                else
                {
                    Console.WriteLine("Customer creation failed.");
                    return null;
                }
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine(" RabbitMQ not reachable, retrying...");
                Thread.Sleep(5000);
                throw new Exception("RabbitMQ is not reachable.", ex);
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
  }
}
