using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CreditProposalService.IntegrationEvents.EventHandling;
using IntegrationEvents.Events;
using Microsoft.Extensions.Options;

public class CreditProposalConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IEventHandler<CreditProposalRequestedEvent> _eventHandler;
    private readonly RabbitMQSettings _settings;

    public CreditProposalConsumer(IEventHandler<CreditProposalRequestedEvent> eventHandler, IOptions<RabbitMQSettings> settings)
    {
        _settings = settings.Value;
        _eventHandler = eventHandler;
        var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        DeclareQueue();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine("Received message: " + content);

            const int maxRetries = 5;
            int retries = GetRetryCount(ea.BasicProperties);
            try
            {
                // Processamento da mensagem
                ProcessMessage(content, ea);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tentativa {retries + 1} falhou: {ex.Message}. Tentando novamente...");
                if (retries < maxRetries)
                {
                    var properties = _channel.CreateBasicProperties();
                    properties.Headers = ea.BasicProperties.Headers ?? new Dictionary<string, object>();
                    properties.Headers["x-retry-count"] = retries + 1;
                    properties.Headers["x-delay"] = 100000;

                    // Reenviar a mensagem para a fila
                    _channel.BasicPublish(exchange: "",
                                         routingKey: "CreditProposalQueue",
                                         basicProperties: properties,
                                         body: ea.Body);
                    _channel.BasicAck(ea.DeliveryTag, false); // Remove a mensagem atual
                }
                else
                {
                    Console.WriteLine("Número máximo de tentativas atingido. Rejeitando mensagem.");
                    _channel.BasicNack(ea.DeliveryTag, false, false); 
                }
            }
        };
        _channel.BasicConsume("CreditProposalQueue", false, consumer);
        return Task.CompletedTask;
    }

    public async void ProcessMessage(string message, BasicDeliverEventArgs  ea)
    {
        try
        {
            var @event = JsonConvert.DeserializeObject<CreditProposalRequestedEvent>(message);
            await _eventHandler.Handle(@event);
        }
        catch (Exception  ex)
        {
            Console.WriteLine($"Error parsing message: {ex.Message}");
            throw new Exception("Falha ao processar a mensagem.");
        }
    }

    static int GetRetryCount(IBasicProperties properties)
    {
        if (properties.Headers != null && properties.Headers.ContainsKey("x-retry-count"))
        {
            return Convert.ToInt32(properties.Headers["x-retry-count"]);
        }
        return 0;
    }

     private void DeclareQueue()
    {
        var dlqQueue = _settings.QueueName + "Dlq";
        var args = new Dictionary<string, object>
        {
            {"x-dead-letter-exchange", ""},
            {"x-dead-letter-routing-key", "CreditProposalQueueDlq"},
            {"x-message-ttl", 60000},  
            {"x-max-length", 10000}, 
            {"x-max-delivery-count", 5}
        };

        _channel.QueueDeclare(queue: "CreditProposalQueue",
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: args);

        _channel.QueueDeclare(queue: "CreditProposalQueueDlq",
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
    } 

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
