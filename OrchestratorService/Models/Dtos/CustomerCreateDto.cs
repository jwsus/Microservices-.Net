namespace OrchestratorService.Models
{
    public class CustomerCreateDto
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public decimal Income { get; set; }
        public int PaymentHistoryScore { get; set; }
    }
}
