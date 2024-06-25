using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrchestratorService.Models;

namespace OrchestratorService.Infrastructure.EntityConfigurations
{
    public class CreditProposalConfiguration : IEntityTypeConfiguration<CreditProposal>
    {
        public void Configure(EntityTypeBuilder<CreditProposal> builder)
        {
            builder.HasKey(cp => cp.Id);
            builder.Property(cp => cp.ProposalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(cp => cp.Status).IsRequired();
            builder.Property(cp => cp.CreationDate).IsRequired();
            builder.Property(cp => cp.LastUpdateDate).IsRequired();

            builder.HasOne(cp => cp.Customer)
                   .WithMany(c => c.CreditProposals)
                   .HasForeignKey(cp => cp.CustomerId);
        }
    }
}
