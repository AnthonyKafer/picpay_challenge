using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace picpay_challenge.Domain.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public string? FullName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public DateTime UpdatedAt = DateTime.UtcNow;
        public string? StoreName { get; set; }
    }
}
