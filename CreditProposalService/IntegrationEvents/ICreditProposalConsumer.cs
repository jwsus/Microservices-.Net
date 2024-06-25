using IntegrationEvents.Events;

namespace EventHandling
{
    public interface ICustomerIntegrationEventService
    {
        void HandleMessage(CreditProposalRequestedEvent @event);
    }
}
