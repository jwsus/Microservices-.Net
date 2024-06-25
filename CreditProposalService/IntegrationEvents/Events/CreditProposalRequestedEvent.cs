namespace IntegrationEvents.Events
{
    public class CreditProposalRequestedEvent
    {
        public Guid Id { get; set; }
        public decimal Income { get; set; }
        public int PaymentHistoryScore { get; set; }
    }
}
