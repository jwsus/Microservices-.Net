namespace IntegrationEvents.Events
{
    public class CustomerRegisteredEvent
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public decimal Income { get; set; }
        public int PaymentHistoryScore { get; set; }
        public DateTime CreationDate { get; set; }

        public CustomerRegisteredEvent(Guid customerId, string name, decimal income, int paymentHistoryScore, DateTime creationDate)
        {
            CustomerId = customerId;
            Name = name;
            Income = income;
            PaymentHistoryScore = paymentHistoryScore;
            CreationDate = creationDate;
        }
    }
}
