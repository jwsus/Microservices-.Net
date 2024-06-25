using System;

namespace OrchestratorService.Models
{
    public class CreditProposal
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal ProposalAmount { get; set; }
        public CreditProposalStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
