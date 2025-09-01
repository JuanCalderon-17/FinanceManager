namespace FinanceManager.Models
{
    // hereis is the model for a transaction
    public class TransactionDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; } 
        public DateTime TransactionDate { get; set; }
        public string Category { get; set; }   

    }
}
