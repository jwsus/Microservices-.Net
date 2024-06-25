using System.Threading.Tasks;
using OrchestratorService.IntegrationEvents.Events;

namespace OrchestratorService.IntegrationEvents
{
    public interface ICustomerIntegrationEventService
    {
        // Task PublishThroughEventBusAsync(CustomerRegisteredIntegrationEvent evt);
        Task<CustomerRegisteredIntegrationEvent> PublishThroughEventBusAsync(CustomerRegisteredIntegrationEvent evt);
    }
}
