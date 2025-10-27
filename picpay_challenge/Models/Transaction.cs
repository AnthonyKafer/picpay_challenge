using System.ComponentModel.DataAnnotations.Schema;
namespace PicPayChallenge.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [Column(TypeName = "numeric(18,5)")]
        public decimal Value { get; set; }

        public int PayerId { get; set; }
        [ForeignKey(nameof(PayerId))] public BaseUser Payer { get; set; }

        public int PayeeId { get; set; }
        [ForeignKey(nameof(PayeeId))] public BaseUser Payee { get; set; }

        public DateTime StartedAt { get; set; }
        public DateTime ConfirmedAt { get; set; }
        public enum Status
        {
            Pending,
            Approved,
            Failed
        }
    }
}