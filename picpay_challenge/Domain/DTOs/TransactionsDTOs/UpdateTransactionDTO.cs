using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace picpay_challenge.Domain.DTOs.TransactionsDTOs
{
    public class UpdateTransactionDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public DateTime UpdatedAt = DateTime.UtcNow;
        public string? CNPJ { get; set; }
        public string? StoreName { get; set; }
    }
}
