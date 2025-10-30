using picpay_challenge.Domain.Models;

namespace picpay_challenge.Domain.DTOs.TransactionsDTOs
{
    public class ServiceResponseTransactionDTO
    {
        public string Message { get; set; } = null!;
        public Transaction? Data { get; set; } = null!;
    }
}
