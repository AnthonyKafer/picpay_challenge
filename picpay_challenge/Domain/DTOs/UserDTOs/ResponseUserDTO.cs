using picpay_challenge.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace picpay_challenge.Domain.DTOs.UserDTOs
{
    public class ResponseUserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CPF { get; set; } = null!;
        public decimal Balance { get; set; }
        public BaseUser.Roles Role { get; set; }
        public string? CNPJ { get; set; }
        public string? StoreName { get; set; }
    }
}
