using Microsoft.EntityFrameworkCore;
using CardIssuanceService.Models;

namespace CustomerService.Data
{
    public class CreditCardContext : DbContext
    {
        public CreditCardContext(DbContextOptions<CreditCardContext> options) : base(options) { }

        public DbSet<CreditCard> CreditCards { get; set; }
    }
}
