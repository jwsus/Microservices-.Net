using Microsoft.EntityFrameworkCore;
using CreditProposalService.Models;

namespace CustomerService.Data
{
    public class CreditProposalContext : DbContext
    {
        public CreditProposalContext(DbContextOptions<CreditProposalContext> options) : base(options) { }

        public DbSet<CreditProposal> CreditProposals { get; set; }
    }
}
