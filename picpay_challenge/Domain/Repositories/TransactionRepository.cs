using Microsoft.EntityFrameworkCore;
using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Helpers;
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

            public List<ResponseTransactionDTO> FindMany(TransactionQuery query)
            {

                var transaction = _context.Transactions
                    .Include(transaction => transaction.Payer)
                    .Include(transaction => transaction.Payee)
                    .AsQueryable();

                if (query.Value != null)
                {
                    transaction = transaction.Where(transaction => transaction.Value == query.Value);
                }
                if (!string.IsNullOrWhiteSpace(query.ReceiverName))
                {
                    transaction = transaction.Where(transaction => transaction.Payee.FullName.Contains(query.ReceiverName));
                }
                if (!string.IsNullOrWhiteSpace(query.PayerName))
                {
                    transaction = transaction.Where(transaction => transaction.Payer.FullName.Contains(query.PayerName));
                }
                if (query.ReceiverId != null)
                {
                    transaction = transaction.Where(transaction => transaction.PayeeId == query.ReceiverId);
                }
                if (query.PayerId != null)
                {
                    transaction = transaction.Where(transaction => transaction.PayerId == query.PayerId);
                }

                if (query.CreatedAt != null)
                {
                    transaction = transaction.Where(transaction => transaction.StartedAt >= query.CreatedAt);
                }
                if (query.UpdatedAt != null)
                {
                    transaction = transaction.Where(transaction => transaction.ConfirmedAt >= query.UpdatedAt);
                }
                if (!string.IsNullOrWhiteSpace(query.SortBy))
                {
                    if (query.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = query.IsDescending ? transaction.OrderByDescending(x => x.StartedAt) : transaction.OrderBy(x => x.StartedAt);
                    }
                    else if (query.SortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = query.IsDescending ? transaction.OrderByDescending(x => x.ConfirmedAt) : transaction.OrderBy(x => x.ConfirmedAt);
                    }
                    else if (query.SortBy.Equals("Value", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = query.IsDescending ? transaction.OrderByDescending(x => x.Value) : transaction.OrderBy(x => x.Value);
                    }

                    else transaction = query.IsDescending ? transaction.OrderByDescending(x => x.Id) : transaction.OrderBy(x => x.Id);
                }


                return transaction.Skip((query.CurrentPage - 1) * query.PageCount).Take(query.PageCount).Select(
                    transaction =>
                new ResponseTransactionDTO()
                {
                    Value = transaction.Value,
                    Payer = new PayerAndPayee()
                    {
                        FullName = transaction.Payer.FullName,
                        Id = transaction.Payer.Id
                    },
                    Payee = new PayerAndPayee()
                    {
                        FullName = transaction.Payee.FullName,
                        Id = transaction.Payee.Id
                    },
                    StartedAt = transaction.StartedAt,
                    ConfirmedAt = transaction.ConfirmedAt,
                    Status = transaction.Status
                }
                    ).ToList();
            }
            public List<ResponseTransactionDTO?> FindByUserId(int UserId)
            {
                List<ResponseTransactionDTO>? transactions = _context.Transactions
                    .Select(transaction =>
                new ResponseTransactionDTO()
                {
                    Value = transaction.Value,
                    Payer = new PayerAndPayee()
                    {
                        FullName = transaction.Payer.FullName,
                        Id = transaction.Payer.Id
                    },
                    Payee = new PayerAndPayee()
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

            public ResponseTransactionDTO? FindById(int Id, int userId)
            {
                var transaction = _context.Transactions
                    .Include(transaction => transaction.Payee)
                    .Include(transaction => transaction.Payer)
                    .FirstOrDefault(x => x.Id == Id && x.PayerId == userId) ?? null;

                if (transaction == null) throw new HttpException(HttpStatusCode.NotFound, "Transaction not found");
                return new ResponseTransactionDTO()
                {
                    Payer = new PayerAndPayee()
                    {
                        FullName = transaction.Payer.FullName,
                        Id = transaction.Payer.Id
                    },
                    Payee = new PayerAndPayee()
                    {
                        FullName = transaction.Payee.FullName,
                        Id = transaction.Payee.Id
                    },
                    Value = transaction.Value,
                    ConfirmedAt = transaction.ConfirmedAt,
                    StartedAt = transaction.StartedAt,
                    Status = transaction.Status
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