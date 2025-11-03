using picpay_challenge.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace picpay_challenge.Domain.DTOs.TransactionsDTOs
{
    public class PayerAndPayee
    {
        public string FullName { get; set; } = String.Empty;
        public int Id { get; set; }

    }
    public class ResponseTransactionDTO
    {
        public decimal Value { get; set; }
        public PayerAndPayee Payer { get; set; }
        public PayerAndPayee Payee { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public Transaction.StatusTypes Status { get; set; }

    }
}
