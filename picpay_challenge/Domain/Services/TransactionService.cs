using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Integrations;
using picpay_challenge.Domain.Mappers.UserMappers;
using picpay_challenge.Domain.Models.Transaction;
using picpay_challenge.Helpers;
using picpay_challenge.Repositories.picpay_challenge.Repositories;
using System.Net;

namespace picpay_challenge.Domain.Services
{
    public class TransactionService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly PaymentExternalAuthorizor _paymentExternalAuthorizor;
        private readonly NotificationExternal _notificationExternal;
        private readonly IMapper _mapper;
        public TransactionService(TransactionRepository transactionRepository,
            PaymentExternalAuthorizor paymentExternalAuthorizor,
            NotificationExternal notificationExternal,
            IMapper mapper
            )
        {
            _transactionRepository = transactionRepository;
            _paymentExternalAuthorizor = paymentExternalAuthorizor;
            _notificationExternal = notificationExternal;
            _mapper = mapper;

        }
        public async Task<ResponseSingleTransactionDTO?> Create([FromServices] UserService userService, CreateTransactionDTO payload, int payerId)
        {
            var payer = await userService.FindById(payerId) ?? null;
            var payee = await userService.FindById(payload.PayeeId) ?? null;
            if (payer == null || payee == null) throw new HttpException(HttpStatusCode.NotFound, "Either payer or payee do not exist");


            if (payer.Balance - payload.Value < 0) throw new HttpException(HttpStatusCode.Unauthorized, "No enough funds");
            DateTime proccessStart = DateTime.UtcNow;



            var paymentAuthorization = await _paymentExternalAuthorizor.IsPaymentAuthorized();

            if (paymentAuthorization == null) throw new HttpException(HttpStatusCode.Unauthorized, "Something went wrong");

            if (!paymentAuthorization.Data.authorization) throw new HttpException(HttpStatusCode.Unauthorized, "Transaction not authorized by external validator");
            Transaction transaction = new Transaction()
            {
                PayeeId = payload.PayeeId,
                PayerId = payerId,
                Value = payload.Value,
                CreatedAt = proccessStart,
                UpdatedAt = DateTime.UtcNow,
                Status = Transaction.StatusTypes.Approved
            };
            var payment = await _transactionRepository.AddAsync(transaction, payer.Id, payee.Id);

            if (payment == null) throw new HttpException(HttpStatusCode.InternalServerError, "Something went wrong while registering the payment");

            await _notificationExternal.SendConfirmNotification();

            return _mapper.Map<ResponseSingleTransactionDTO>(payment);
        }
        public async Task<ResponseListTransactionDTO> GetUserTransactions(TransactionQuery query, int id)
        {
            var userTransactions = await _transactionRepository.FindByUserId(query, id);
            return _mapper.Map<ResponseListTransactionDTO>(userTransactions);
        }
        public async Task<ResponseListTransactionDTO> FindMany(TransactionQuery query)
        {
            var transactions = await _transactionRepository.GetAllAsync(query);
            return _mapper.Map<ResponseListTransactionDTO>(transactions);
        }

        public async Task<Transaction?> FindById(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            return _mapper.Map<Transaction>(transaction);
        }

    }
}
