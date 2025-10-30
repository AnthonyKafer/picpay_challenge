using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Repositories.Interfaces;

namespace picpay_challenge.Repositories
{
    namespace picpay_challenge.Repositories
    {
        public class TransactionRepository : IRepositoryInterface<Transaction>
        {
            private readonly AppDbContext _context;

            public TransactionRepository(AppDbContext context)
            {
                _context = context;
            }

            public List<Transaction> FindMany()
            {
                List<Transaction>? transactions = _context.Transactions.ToList() ?? null;
                return transactions ?? null;
            }
            public Transaction? FindById(int Id)
            {
                var transatiction = _context.Transactions.FirstOrDefault(x => x.Id == Id) ?? null;
                if (transatiction == null) return null;
                return transatiction;
            }
            public Transaction? ContestTransaction(int Id)
            {
                var transatiction = _context.Transactions.FirstOrDefault(x => x.Id == Id) ?? null;
                if (transatiction == null) return null;
                transatiction.Status = Transaction.StatusTypes.Contested;
                _context.SaveChanges();

                return transatiction;

            }
            public Transaction Create(Transaction payload)
            {
                _context.Transactions.Add(payload);
                _context.SaveChanges();
                return payload;
            }

        }

    }
}