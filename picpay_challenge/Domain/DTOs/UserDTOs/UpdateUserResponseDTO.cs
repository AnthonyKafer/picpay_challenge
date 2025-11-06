using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace picpay_challenge.Domain.DTOs.UserDTOs
{
    public class UpdateUserResponseDTO : ResponseSingleUserDTO
    {
        public DateTime UpdatedAt = DateTime.UtcNow;
    }
}
