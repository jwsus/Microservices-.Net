namespace OrchestratorService.IntegrationEvents.Events
{
    public class CustomerRegisteredIntegrationEvent
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public decimal Income { get; set; }
        public int PaymentHistoryScore { get; set; }
        public DateTime CreationDate { get; set; }

        public CustomerRegisteredIntegrationEvent(Guid customerId, string name, decimal income, int paymentHistoryScore, DateTime creationDate)
        {
            CustomerId = customerId;
            Name = name;
            Income = income;
            PaymentHistoryScore = paymentHistoryScore;
            CreationDate = creationDate;
        }
    }
}
