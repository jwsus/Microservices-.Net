namespace CustomerRegistrationService.IntegrationEvents.EventHandling
{
    public interface IEventHandler<T>
    {
        Task Handle(T @event);
    }
}
