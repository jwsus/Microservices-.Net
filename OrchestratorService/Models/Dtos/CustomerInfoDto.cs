public class CustomerInfoDto
{
    public Guid CustomerId { get; set; }
    public decimal CreditProposalAmount { get; set; }
    public List<CreditCardDto> CreditCards { get; set; }
}

public class CreditCardDto
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
}
