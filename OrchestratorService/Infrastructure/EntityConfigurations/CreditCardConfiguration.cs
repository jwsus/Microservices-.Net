using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrchestratorService.Models;

namespace OrchestratorService.Infrastructure.EntityConfigurations
{
    public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        { 
          
            builder.HasKey(cc => cc.Id);
            builder.Property(cc => cc.CardNumber).IsRequired().HasMaxLength(20);
            builder.Property(cc => cc.ExpiryDate).IsRequired();
            builder.Property(cc => cc.CVV).IsRequired().HasMaxLength(3);
            builder.Property(cc => cc.Status).IsRequired();
            builder.Property(cc => cc.IssuanceDate).IsRequired();

            builder.HasOne(cc => cc.Customer)
                   .WithMany(c => c.CreditCards)
                   .HasForeignKey(cc => cc.CustomerId);
        }
    }
}
