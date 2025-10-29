using picpay_challenge.DTOs.UserDTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace picpay_challenge.Domain.Models
{
    public class BaseUser
    {

        public int Id { get; set; }
        public enum UserTypes
        {
            User,
            Storekeeper
        }
        [Required]
        public UserTypes UserType { get; set; }

        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string CPF { get; set; } = null!;
        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now!;
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "numeric(18,5)")]
        public decimal Balance { get; set; }
        public string? CNPJ { get; set; } = null;
        public string? StoreName { get; set; } = null;

    }
}