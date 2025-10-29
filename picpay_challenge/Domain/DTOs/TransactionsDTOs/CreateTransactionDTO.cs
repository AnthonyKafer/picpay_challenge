using PicPayChallenge.Models;
using System.ComponentModel.DataAnnotations;
namespace picpay_challenge.Domain.DTOs.TransactionsDTOs
{
    public class CreateTransactionDTO
    {
        public int PayerId { get; set; }
        public int PayeeId { get; set; }
        public decimal Value { get; set; }
    }
}