namespace PicPayChallenge.Models
{
    public class StoreKeeper : BaseUser
    {
        public string CNPJ { get; set; } = null!;
        public string StoreName { get; set; } = null!;
    }
}