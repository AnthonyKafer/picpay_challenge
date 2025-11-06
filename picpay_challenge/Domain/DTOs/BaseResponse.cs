namespace picpay_challenge.Domain.DTOs
{
    public class BaseResponse<DTO>
    {
        public List<DTO> Data { get; set; } = [];
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
    public class BaseSingleResponse
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt
        {
            get; set;

        }
    }
}
