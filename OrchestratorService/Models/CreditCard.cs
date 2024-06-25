using System;

namespace OrchestratorService.Models
{
    public class CreditCard
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CVV { get; set; }
        public string Status { get; set; }
        public DateTime IssuanceDate { get; set; }
        public virtual Customer Customer { get; set; }
        public decimal Amount { get; set; }
    }
}
