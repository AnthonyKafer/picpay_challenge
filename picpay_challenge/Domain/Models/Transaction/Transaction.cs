using picpay_challenge.Domain.Models.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace picpay_challenge.Domain.Models.Transaction
{
    public class Transaction : EntityBase
    {
        [Column(TypeName = "numeric(18,5)")]
        public decimal Value { get; set; }

        public int PayerId { get; set; }
        [ForeignKey(nameof(PayerId))] public BaseUser Payer { get; set; } = null!;

        public int PayeeId { get; set; }
        [ForeignKey(nameof(PayeeId))] public BaseUser Payee { get; set; } = null!;

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