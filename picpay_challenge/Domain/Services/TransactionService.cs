using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Integrations;
using picpay_challenge.Domain.Models;
using picpay_challenge.Repositories.picpay_challenge.Repositories;
using System.Net;

namespace picpay_challenge.Domain.Services
{
    public class TransactionService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly PaymentExternalAuthorizor _paymentExternalAuthorizor;
        private readonly NotificationExternal _notificationExternal;
        public TransactionService(TransactionRepository userRepository, PaymentExternalAuthorizor paymentExternalAuthorizor, NotificationExternal notificationExternal)
        {
            _transactionRepository = userRepository;
            _paymentExternalAuthorizor = paymentExternalAuthorizor;
            _notificationExternal = notificationExternal;

        }
        public async Task<ResponseTransactionDTO?> Create([FromServices] UserService userService, CreateTransactionDTO payload)
        {
            var payer = userService.FindById(payload.PayerId) ?? null;
            var payee = userService.FindById(payload.PayeeId) ?? null;
            if (payer == null || payee == null) throw new HttpException(HttpStatusCode.NotFound, "Either payer or payee do not exist");
            if (payer.Role != BaseUser.Roles.User) throw new HttpException(HttpStatusCode.Unauthorized, "Only regular users can pay transfers");

            if (payer.Balance - payload.Value < 0) throw new HttpException(HttpStatusCode.Unauthorized, "No enough funds");
            DateTime proccessStart = DateTime.UtcNow;



            var paymentAuthorization = await _paymentExternalAuthorizor.IsPaymentAuthorized();

            if (paymentAuthorization == null) throw new HttpException(HttpStatusCode.Unauthorized, "Something went wrong");

            if (!paymentAuthorization.Data.authorization) throw new HttpException(HttpStatusCode.Unauthorized, "Transaction not authorized by external validator");
            Transaction transaction = new Transaction()
            {
                PayeeId = payload.PayeeId,
                PayerId = payload.PayerId,
                Value = payload.Value,
                StartedAt = proccessStart,
                ConfirmedAt = DateTime.UtcNow,
                Status = Transaction.StatusTypes.Approved
            };
            var payment = await _transactionRepository.Create(transaction, payer.Id, payee.Id);

            if (payment == null) throw new HttpException(HttpStatusCode.InternalServerError, "Something went wrong while registering the payment");

            await _notificationExternal.SendConfirmNotification();

            return payment;
        }
        public List<ResponseTransactionDTO?> GetUserTransactions([FromServices] UserService userService, int id, string userEmail)
        {
            var user = userService.FindById(id) ?? null;
            if (user == null) throw new HttpException(HttpStatusCode.NotFound, "User does not exist");
            if (user.Email != userEmail) throw new HttpException(HttpStatusCode.Unauthorized, "You cannot access registers that are not associated with your account");

            return _transactionRepository.FindByUserId(id);
        }
        public List<ResponseTransactionDTO?>? FindMany()
        {
            return _transactionRepository.FindMany();
        }

        public ResponseTransactionDTO? FindById(int id, int userId)
        {
            return _transactionRepository.FindById(id, userId);
        }

    }
}
