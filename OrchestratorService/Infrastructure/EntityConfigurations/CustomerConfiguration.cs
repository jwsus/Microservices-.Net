using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrchestratorService.Models;

namespace OrchestratorService.Infrastructure.EntityConfigurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Income).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(c => c.PaymentHistoryScore).IsRequired();
            builder.Property(c => c.CreationDate).IsRequired();

            builder.HasMany(c => c.CreditProposals)
                   .WithOne(cp => cp.Customer)
                   .HasForeignKey(cp => cp.CustomerId);

            builder.HasMany(c => c.CreditCards)
                   .WithOne(cc => cc.Customer)
                   .HasForeignKey(cc => cc.CustomerId);
        }
    }
}
