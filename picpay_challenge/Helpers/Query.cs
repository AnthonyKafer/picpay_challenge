using picpay_challenge.Domain.Models;

namespace picpay_challenge.Helpers
{
    public class BaseQuery
    {
        public string? SortBy { get; set; }
        public bool IsDescending = false;
        public int? Page { get; set; }
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; } = 10;

    }
    public class TransactionQuery : BaseQuery
    {
        public int? ReceiverId { get; set; }
        public int? PayerId { get; set; }
        public decimal? Value { get; set; }
        public string? PayerName { get; set; }
        public string? ReceiverName { get; set; }
    }
    public class UserQuery : BaseQuery
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? CPF { get; set; }
        public string? CNPJ { get; set; }
        public BaseUser.Roles? Role { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
