using System.ComponentModel.DataAnnotations;

namespace picpay_challenge.Domain.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public string? FullName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public string? CPF { get; set; } = null!;
        public string? CNPJ { get; set; }
        public string? StoreName { get; set; }
    }
}
