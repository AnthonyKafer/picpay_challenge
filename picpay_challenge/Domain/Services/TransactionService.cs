using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Services.Interfaces;
using picpay_challenge.DTOs.UserDTOs;
using picpay_challenge.Repositories;
using picpay_challenge.Repositories.picpay_challenge.Repositories;

namespace picpay_challenge.Domain.Services
{
    public class TransactionService : IServiceInterface<Transaction>
    {
        private readonly TransactionRepository _transactionRepository;

        public TransactionService(TransactionRepository userRepository)
        {
            _transactionRepository = userRepository;
        }
        public Transaction Create(Transaction payload)
        {

            return _transactionRepository.Create(payload);
        }
        public List<Transaction>? FindMany()
        {
            return _transactionRepository.FindMany();
        }
        public Transaction? FindById(int id)
        {
            return _transactionRepository.FindById(id);
        }
        public Transaction? ContestTransaction(int id)
        {
            return _transactionRepository.ContestTransaction(id);
        }
    }
}
