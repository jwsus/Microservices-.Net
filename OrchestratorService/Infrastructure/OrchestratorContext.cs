using Microsoft.EntityFrameworkCore;
using OrchestratorService.Infrastructure.EntityConfigurations;
using OrchestratorService.Models;

namespace OrchestratorService.Data
{
    public class OrchestratorContext : DbContext
    {
        public OrchestratorContext(DbContextOptions<OrchestratorContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CreditProposal> CreditProposals { get; set; }
        public DbSet<CardIssuedEvent> CardIssuedEvents { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CreditProposalConfiguration());
            modelBuilder.ApplyConfiguration(new CreditCardConfiguration());
        }
    }
}
