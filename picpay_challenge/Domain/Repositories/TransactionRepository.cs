using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using System.Net;

namespace picpay_challenge.Repositories
{
    namespace picpay_challenge.Repositories
    {
        public class TransactionRepository
        {
            private readonly AppDbContext _context;

            public TransactionRepository(AppDbContext context)
            {
                _context = context;
            }

            public List<ResponseTransactionDTO?>? FindMany()
            {
                List<ResponseTransactionDTO>? transactions = _context.Transactions.Select(transaction =>
                new ResponseTransactionDTO()
                {
                    Value = transaction.Value,
                    Payer = new PayerAndPayee
                    {
                        FullName = transaction.Payer.FullName,
                        Id = transaction.Payer.Id
                    },
                    Payee = new PayerAndPayee
                    {
                        FullName = transaction.Payee.FullName,
                        Id = transaction.Payee.Id
                    },
                    StartedAt = transaction.StartedAt,
                    ConfirmedAt = transaction.ConfirmedAt,
                    Status = transaction.Status
                }
                ).ToList() ?? null;
                return transactions;
            }
            public List<ResponseTransactionDTO?> FindByUserId(int UserId)
            {
                List<ResponseTransactionDTO>? transactions = _context.Transactions
                    .Select(transaction =>
                new ResponseTransactionDTO()
                {
                    Value = transaction.Value,
                    Payer = new PayerAndPayee
                    {
                        FullName = transaction.Payer.FullName,
                        Id = transaction.Payer.Id
                    },
                    Payee = new PayerAndPayee
                    {
                        FullName = transaction.Payee.FullName,
                        Id = transaction.Payee.Id
                    },
                    StartedAt = transaction.StartedAt,
                    ConfirmedAt = transaction.ConfirmedAt,
                    Status = transaction.Status
                }
                )
                 .Where(x => x.Payer.Id == UserId).ToList() ?? null;
                if (transactions == null) return null;
                return transactions;
            }

            public ResponseTransactionDTO? FindById(int Id)
            {
                var transatiction = _context.Transactions.FirstOrDefault(x => x.Id == Id) ?? null;

                if (transatiction == null) return null;
                return new ResponseTransactionDTO()
                {
                    Payer = new PayerAndPayee
                    {
                        FullName = transatiction.Payer.FullName,
                        Id = transatiction.Payer.Id
                    },
                    Payee = new PayerAndPayee
                    {
                        FullName = transatiction.Payee.FullName,
                        Id = transatiction.Payee.Id
                    },
                    Value = transatiction.Value,
                    ConfirmedAt = transatiction.ConfirmedAt,
                    StartedAt = transatiction.StartedAt,
                    Status = transatiction.Status
                };
            }
            public async Task<ResponseTransactionDTO?> Create(Transaction payload, int payerId, int payeeId)
            {
                var transactionDb = _context.Database.BeginTransaction();
                try
                {
                    var payer = _context.Users.FirstOrDefault(x => x.Id == payerId);
                    payer.Balance -= payload.Value;

                    var payee = _context.Users.FirstOrDefault(x => x.Id == payeeId);
                    payee.Balance += payload.Value;

                    var payment = _context.Transactions.Add(payload);
                    await _context.SaveChangesAsync();
                    await transactionDb.CommitAsync();

                    return new ResponseTransactionDTO()
                    {
                        Payer = new PayerAndPayee()
                        {
                            FullName = payer.FullName,
                            Id = payer.Id
                        },
                        Payee = new PayerAndPayee()
                        {
                            FullName = payee.FullName,
                            Id = payee.Id
                        },
                        Value = payload.Value,
                        ConfirmedAt = DateTime.Now,
                        StartedAt = payload.StartedAt,
                        Status = Transaction.StatusTypes.Approved
                    };

                }
                catch (Exception ex)
                {
                    await transactionDb.RollbackAsync();
                    throw new HttpException(HttpStatusCode.InternalServerError, "Something went wrong at registering the transaction, try again later");
                }

            }

        }

    }
}