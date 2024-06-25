namespace IntegrationEvents.Events
{
    public class CardIssuanceRequestedEvent
    {
        public Guid CustomerId { get; set; }
        public decimal ProposalAmount { get; set; }
    }
}
