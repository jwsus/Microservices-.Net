using CustomerService.Data;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using IntegrationEvents.Events;
using CreditProposalService.Models;

namespace CreditProposalService.IntegrationEvents.EventHandling
{
    public class CreditProposalEventHandler : IEventHandler<CreditProposalRequestedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessagePublisher _messagePublisher;
        private readonly RabbitMQSettings _settings;

        public CreditProposalEventHandler(IServiceProvider serviceProvider, MessagePublisher messagePublisher, IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
        }
        public async Task Handle(CreditProposalRequestedEvent @event)
        {

            var approvedAmount = CalculateApprovedAmount(@event.Income, @event.PaymentHistoryScore);
            
            var newCreditProposal = new CreditProposal
            {
                Id = Guid.NewGuid(),
                CustomerId = @event.Id,
                ProposalAmount = approvedAmount, 
                Status = approvedAmount > 0 ?  CreditProposalStatus.Approved :  CreditProposalStatus.Rejected,
                CreationDate = DateTime.UtcNow,
                LastUpdateDate = DateTime.UtcNow
            };

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CreditProposalContext>();

                context.CreditProposals.Add(newCreditProposal);
                await context.SaveChangesAsync();
            }

            if(newCreditProposal.Status == CreditProposalStatus.Approved)
            {
                _messagePublisher.Publish(newCreditProposal, _settings.SendQueue);
            }
            
        }

        private decimal CalculateApprovedAmount(decimal income, int paymentHistoryScore)
        {
            if (paymentHistoryScore > 500 && income > 5000)
            {
                return income * 2.5m;
            }
            else if (paymentHistoryScore > 250)
            {
                return income * 0.25m; 
            }
            else
            {
                return 0; 
            }
        }
    }
}
