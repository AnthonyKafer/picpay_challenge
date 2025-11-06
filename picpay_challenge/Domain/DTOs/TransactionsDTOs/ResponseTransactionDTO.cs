using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Models.Transaction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace picpay_challenge.Domain.DTOs.TransactionsDTOs
{
    public class PayerAndPayee
    {
        public string FullName { get; set; } = String.Empty;
        public int Id { get; set; }

    }
    public class ResponseSingleTransactionDTO : EntityBase
    {
        public decimal Value { get; set; }

        public PayerAndPayee Payer { get; set; }
        public PayerAndPayee Payee { get; set; }
        public Transaction.StatusTypes Status { get; set; }

    }
    public class ResponseListTransactionDTO : BaseResponse<ResponseSingleTransactionDTO> { }
}
