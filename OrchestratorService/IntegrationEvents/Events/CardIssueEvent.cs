using System;

namespace OrchestratorService.Models
{
    public class CardIssuedEvent
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CardNumber { get; set; }
        public DateTime IssuanceDate { get; set; }
    }
}
