using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.DTOs;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.Mappers.UserMappers;
using picpay_challenge.Domain.Models.Transaction;
using picpay_challenge.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace picpay_challenge.Repositories
{
    namespace picpay_challenge.Repositories
    {
        public class TransactionRepository : RepositoryBase<Transaction, TransactionQuery>
        {
            private readonly AppDbContext _context;
            protected readonly DbSet<Transaction> _dbSet;


            public TransactionRepository(AppDbContext context) : base(context)
            {
                _context = context;
                _dbSet = context.Set<Transaction>();

            }
            public async Task<BaseResponse<Transaction>> FindByUserId(TransactionQuery filter, int userId)
            {
                IQueryable<Transaction> query = _dbSet;
                int total = (int)Math.Ceiling(Convert.ToDecimal(query.Count()) / Convert.ToDecimal(filter.PageCount));

                query = query.Where(x => x.PayerId == userId);
                List<Transaction> data = await base.ApplyOrdering(ApplyFilter(query, filter), filter)
                    .Include(x => x.Payee)
                    .Include(x => x.Payer)
                    .Skip((filter.CurrentPage - 1) * filter.PageCount)
                    .Take(filter.PageCount)
                    .ToListAsync();

                base.ApplyOrdering(query, filter);
                return new BaseResponse<Transaction>()
                {
                    Data = data,
                    CurrentPage = filter.CurrentPage,
                    TotalPages = total == 0 ? 1 : total,
                };
            }

            override public async Task<BaseResponse<Transaction>> GetAllAsync(TransactionQuery filter)
            {
                IQueryable<Transaction> query = _dbSet;
                int total = (int)Math.Ceiling(Convert.ToDecimal(query.Count()) / Convert.ToDecimal(filter.PageCount));

                List<Transaction> data = await base.ApplyOrdering(ApplyFilter(query, filter), filter)
                    .Include(x => x.Payee)
                    .Include(x => x.Payer)
                    .Skip((filter.CurrentPage - 1) * filter.PageCount)
                    .Take(filter.PageCount)
                    .ToListAsync();
                return new BaseResponse<Transaction>()
                {
                    Data = data,
                    CurrentPage = filter.CurrentPage,
                    TotalPages = total == 0 ? 1 : total,
                };
            }
            public async Task<ResponseSingleTransactionDTO?> AddAsync(Transaction payload, int payerId, int PayeeId)
            {
                var transactionDb = _context.Database.BeginTransaction();
                try
                {
                    var payer = _context.Users.FirstOrDefault(x => x.Id == payerId);
                    payer.Balance -= payload.Value;

                    var payee = _context.Users.FirstOrDefault(x => x.Id == PayeeId);
                    payee.Balance += payload.Value;

                    var payment = await _context.Transactions.AddAsync(payload);
                    await _context.SaveChangesAsync();
                    await transactionDb.CommitAsync();
                    return new ResponseSingleTransactionDTO()
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
                        CreatedAt = payload.CreatedAt,
                        UpdatedAt = payload.UpdatedAt,
                        Status = Transaction.StatusTypes.Approved
                    };

                }
                catch
                {
                    await transactionDb.RollbackAsync();
                    throw;
                }
            }
            public override IQueryable<Transaction> ApplyFilter(IQueryable<Transaction> query, TransactionQuery filter)
            {
                query = query.Include(x => x.Payee);
                query = query.Include(x => x.Payer);

                base.ApplyFilter(query, filter);
                if (!String.IsNullOrEmpty(filter.PayerName))
                {
                    query = query.Where(transaction => transaction.Payer.FullName.Contains(filter.PayerName!));
                }

                if (!String.IsNullOrEmpty(filter.ReceiverName))
                {
                    query = query.Where(transaction => transaction.Payee.FullName.Contains(filter.ReceiverName!));
                }
                if (filter.Value.HasValue)
                {
                    query = query.Where(transaction => transaction.Value == filter.Value);
                }
                if (filter.PayerId.HasValue)
                {
                    query = query.Where(transaction => transaction.PayerId == filter.PayerId);
                }
                if (filter.ReceiverId.HasValue)
                {
                    query = query.Where(transaction => transaction.PayeeId == filter.ReceiverId);
                }

                return query;


            }



        }

    }
}