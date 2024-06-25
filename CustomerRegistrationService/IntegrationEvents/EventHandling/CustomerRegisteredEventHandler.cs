using CustomerRegistrationServiceService.Models;
using CustomerService.Data;
using IntegrationEvents.Events;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CustomerRegistrationService.IntegrationEvents.EventHandling
{
    public class CustomerRegisteredEventHandler : IEventHandler<CustomerRegisteredEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessagePublisher _messagePublisher;
        private readonly RabbitMQSettings _settings;

        public CustomerRegisteredEventHandler(IServiceProvider serviceProvider, MessagePublisher messagePublisher, IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
        }
        public async Task Handle(CustomerRegisteredEvent @event)
        {
            Console.WriteLine($"Cadastrado com sucesso. Id: {@event.CustomerId}, Name: {@event.Name}, Income: {@event.Income}, PaymentHistoryScore: {@event.PaymentHistoryScore}, CreationDate: {@event.CreationDate}");
            
            var customer = new Customer
                {
                    Id = @event.CustomerId,
                    Name = @event.Name,
                    Income = @event.Income,
                    PaymentHistoryScore = @event.PaymentHistoryScore,
                    CreationDate = @event.CreationDate
                };

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CustomerContext>();

                

                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }

            Console.WriteLine($"Cadastrado com sucesso. Id: {@event.CustomerId}");

            _messagePublisher.Publish(customer, _settings.SendQueue);
        }
    }
}
