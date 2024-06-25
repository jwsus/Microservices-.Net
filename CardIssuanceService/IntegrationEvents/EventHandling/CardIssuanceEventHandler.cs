using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using IntegrationEvents.Events;
using CardIssuanceService.Models;
using CustomerService.Data;

namespace CardIssuanceService.IntegrationEvents.EventHandling
{
    public class CardIssuanceEventHandler : IEventHandler<CardIssuanceRequestedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessagePublisher _messagePublisher;
        private readonly RabbitMQSettings _settings;
        private static readonly Random _random = new Random();

        public CardIssuanceEventHandler(IServiceProvider serviceProvider, MessagePublisher messagePublisher, IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
        }
        public async Task Handle(CardIssuanceRequestedEvent @event)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CreditCardContext>();

                var cards = GenerateCreditCards(@event.CustomerId, @event.ProposalAmount);
                context.CreditCards.AddRange(cards);
                await context.SaveChangesAsync();
            }

            _messagePublisher.Publish(@event, _settings.SendQueue);
        }

        private int CalculateNumberOfCards(decimal proposalAmount)
        {
            if (proposalAmount <= 1000)
            {
                return 1;
            }
            else if (proposalAmount <= 5000)
            {
                return 2;
            }
            else if (proposalAmount <= 10000)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        private CreditCard GenerateCreditCard(Guid customerId, decimal amountPerCard)
        {
            return new CreditCard
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                CardNumber = GenerateCardNumber(),
                ExpiryDate = DateTime.UtcNow.AddYears(5),
                CVV = GenerateCVV(),
                Status = "Active",
                IssuanceDate = DateTime.UtcNow,
                Amount = amountPerCard
            };
        }

        private CreditCard[] GenerateCreditCards(Guid customerId, decimal proposalAmount)
        {
            int numberOfCards = CalculateNumberOfCards(proposalAmount);
            decimal amountPerCard = proposalAmount / numberOfCards;

            var cards = new CreditCard[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
            {
                cards[i] = GenerateCreditCard(customerId, amountPerCard);
            }
            return cards;
        }

        private string GenerateCardNumber()
        {
            return string.Concat(Enumerable.Range(0, 16).Select(_ => _random.Next(0, 10).ToString()));
        }

        private string GenerateCVV()
        {
            return string.Concat(Enumerable.Range(0, 3).Select(_ => _random.Next(0, 10).ToString()));
        }
    }
}
