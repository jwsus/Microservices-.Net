using System;

namespace CardIssuanceService.Models
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
        public Decimal Amount { get; set; }
    }
}
