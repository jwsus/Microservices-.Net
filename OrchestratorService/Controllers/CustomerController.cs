using Microsoft.AspNetCore.Mvc;
using OrchestratorService.Data;
using OrchestratorService.IntegrationEvents;
using OrchestratorService.IntegrationEvents.Events;
using OrchestratorService.Models;
using Microsoft.EntityFrameworkCore;

namespace OrchestratorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly OrchestratorContext _context;
        private readonly ICustomerIntegrationEventService _integrationEventService;
         private readonly ICustomerIntegrationEventService _customerIntegrationEventService;

        public CustomerController(OrchestratorContext context, ICustomerIntegrationEventService integrationEventService, ICustomerIntegrationEventService customerIntegrationEventService)
        {
            _context = context;
            _integrationEventService = integrationEventService;
            _customerIntegrationEventService = customerIntegrationEventService;
        }
        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerRegisteredIntegrationEvent customerEvent)
        {
            try
            {
                var result = await _customerIntegrationEventService.PublishThroughEventBusAsync(customerEvent);

                if (result != null && result.CustomerId != Guid.Empty)
                {
                    return Ok(new { message = "Customer created successfully", customerId = result.CustomerId });
                }
                else
                {
                    return StatusCode(500, "Customer creation failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateDatabase()
        {
            try
            {
                await _context.Database.MigrateAsync();
                return Ok("Database updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [HttpGet("info/{customerId}")]
        public async Task<IActionResult> GetCustomerInfo(Guid customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.CreditProposals)
                .Include(c => c.CreditCards)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            var customerInfo = new CustomerInfoDto
            {
                CustomerId = customer.Id,
                CreditProposalAmount = customer.CreditProposals.Sum(cp => cp.ProposalAmount),
                CreditCards = customer.CreditCards.Select(cc => new CreditCardDto
                {
                    Id = cc.Id,
                    CardNumber = cc.CardNumber,
                    ExpiryDate = cc.ExpiryDate,
                    Status = cc.Status,
                    Amount = cc.Amount
                }).ToList()
            };

            return Ok(customerInfo);
        }
    }
}
