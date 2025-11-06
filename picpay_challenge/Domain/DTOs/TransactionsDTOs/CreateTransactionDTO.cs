namespace picpay_challenge.Domain.DTOs.TransactionsDTOs
{
    public class CreateTransactionDTO
    {
        public int PayeeId { get; set; }
        public decimal Value { get; set; }
    }
}