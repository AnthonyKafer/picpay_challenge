using picpay_challenge.Domain.Models.User;
using System.ComponentModel.DataAnnotations;

namespace picpay_challenge.Domain.DTOs.UserDTOs
{
    public class ResponseSingleUserDTO
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

    public class ResponseListUserDTO : BaseResponse<ResponseSingleUserDTO> { }
}
