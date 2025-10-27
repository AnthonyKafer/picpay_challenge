using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicPayChallenge.Models
{
    public abstract class BaseUser
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string CPF { get; set; } = null!;
        [Required]
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "numeric(18,5)")]
        public decimal Balance { get; set; }
    }
}