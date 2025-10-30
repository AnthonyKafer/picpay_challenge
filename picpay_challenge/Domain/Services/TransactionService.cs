using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.Integrations;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Services.Interfaces;
using picpay_challenge.Repositories.picpay_challenge.Repositories;

namespace picpay_challenge.Domain.Services
{
    public class TransactionService : IServiceInterface<Transaction>
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly PaymentExternalAuthorizor _paymentExternalAuthorizor;

        public TransactionService(TransactionRepository userRepository, PaymentExternalAuthorizor paymentExternalAuthorizor)
        {
            _transactionRepository = userRepository;
            _paymentExternalAuthorizor = paymentExternalAuthorizor;
        }
        public async Task<ServiceResponseTransactionDTO?> Create([FromServices] UserService userService, CreateTransactionDTO payload)
        {
            var payer = userService.FindById(payload.PayerId) ?? null;
            var payee = userService.FindById(payload.PayeeId) ?? null;
            if (payer == null || payee == null) return new ServiceResponseTransactionDTO
            {
                Message = "Either payer or payee do not exist",
                Data = null,
            };
            if (payer.UserType == BaseUser.UserTypes.Storekeeper) return new ServiceResponseTransactionDTO
            {
                Message = "An storekeeper cannot pay transfers",
                Data = null,
            }
            ;
            if (payer.Balance - payload.Value < 0) return new ServiceResponseTransactionDTO
            {
                Message = "Not enough funds",
                Data = null,
            };

            Transaction transaction = new Transaction
            {
                PayeeId = payload.PayeeId,
                PayerId = payload.PayerId,
                Value = payload.Value,
                StartedAt = DateTime.UtcNow,
                ConfirmedAt = DateTime.UtcNow,
            };

            var paymentAuthorization = await _paymentExternalAuthorizor.IsPaymentAuthorized();

            if (paymentAuthorization == null) return new ServiceResponseTransactionDTO
            {
                Message = "Something went wrong",
                Data = null,
            };
            if (!paymentAuthorization.Data.authorization) return new ServiceResponseTransactionDTO
            {
                Message = "Transaction not authorized by external validator",
                Data = null,
            };

            var payment = _transactionRepository.Create(transaction);
            userService.ChangeUserBalance(payee.Id, payload.Value);
            userService.ChangeUserBalance(payer.Id, -payload.Value);
            if (payment == null) return null;
            return new ServiceResponseTransactionDTO
            {
                Message = "Succes",
                Data = payment,
            };
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
