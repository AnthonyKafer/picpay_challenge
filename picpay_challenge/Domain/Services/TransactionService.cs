using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
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
        public Transaction Create([FromServices] UserService userService, CreateTransactionDTO payload)
        {
            var payer = userService.FindById(payload.PayerId);
            var payee = userService.FindById(payload.PayeeId);
            //if (payer == null || payee == null) return BadRequest("Payer or payee not found");
            //if (payer.UserType == BaseUser.UserTypes.Storekeeper) return BadRequest("You cannot make transfers from an storekeeper account");
            Transaction transaction = new Transaction
            {
                PayeeId = payload.PayeeId,
                PayerId = payload.PayerId,
            }

            var payment = _transactionRepository.Create(payload);

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
