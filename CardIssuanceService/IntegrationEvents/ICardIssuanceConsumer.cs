using IntegrationEvents.Events;

namespace EventHandling
{
    public interface ICustomerIntegrationEventService
    {
        void HandleMessage(CardIssuanceRequestedEvent @event);
    }
}
