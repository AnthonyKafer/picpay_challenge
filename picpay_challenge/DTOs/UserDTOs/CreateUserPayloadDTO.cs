using PicPayChallenge.Models;
using System.ComponentModel.DataAnnotations;
namespace picpay_challenge.DTOs.UserDTOs
{
    public class CreateUserPayloadDTO
    {
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string CPF { get; set; } = null!;
        [Range(0.01, 100000.0000)]
        public decimal Balance { get; set; }
        public string? CNPJ { get; set; }
        public string? StoreName { get; set; }
    }
}