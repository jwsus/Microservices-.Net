using System;

namespace CustomerRegistrationServiceService.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Income { get; set; }
        public int PaymentHistoryScore { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
