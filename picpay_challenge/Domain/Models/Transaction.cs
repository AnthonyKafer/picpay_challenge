using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace picpay_challenge.Domain.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [Column(TypeName = "numeric(18,5)")]
        public decimal Value { get; set; }

        public int PayerId { get; set; }
        [ForeignKey(nameof(PayerId))] public BaseUser Payer { get; set; } = null!;

        public int PayeeId { get; set; }
        [ForeignKey(nameof(PayeeId))] public BaseUser Payee { get; set; } = null!;

        public DateTime StartedAt { get; set; }
        public DateTime ConfirmedAt { get; set; }
        public enum StatusTypes
        {
            Pending,
            Approved,
            Failed,
            Contested
        }
        [Required]
        public StatusTypes Status { get; set; } = StatusTypes.Pending;

    }
}