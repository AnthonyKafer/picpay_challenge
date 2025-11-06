using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models.User;
using picpay_challenge.Helpers;
using System.Net;

namespace picpay_challenge.Domain.Repositories
{
    public class UserRepository : RepositoryBase<BaseUser, UserQuery>
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<BaseUser?> GetUserCredentials(string Email)
        {
            var userCredentials = await _context.Users.FirstOrDefaultAsync(x => x.Email == Email);
            return userCredentials ?? null;
        }
        public override IQueryable<BaseUser> ApplyFilter(IQueryable<BaseUser> query, UserQuery filter)
        {
            base.ApplyFilter(query, filter);
            if (!String.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(user => user.FullName.Contains(filter.FullName!));
            }
            if (filter.Balance.HasValue)
            {
                query = query.Where(user => user.Balance == filter.Balance);
            }
            if (!String.IsNullOrEmpty(filter.CPF))

            {
                query = query.Where(user => user.CPF.Contains(filter.CPF!));
            }
            if (!String.IsNullOrEmpty(filter.CNPJ))
            {
                query = query.Where(user => user.CNPJ == null ? false : user.CNPJ.Contains(filter.CNPJ));
            }
            if (filter.Role.HasValue)
            {
                query = query.Where(user => user.Role == filter.Role);
            }
            query = query.Where(user => user.IsActive == filter.IsActive);
            query = ApplyOrdering(query, filter);

            return query;


        }
        public async Task<BaseUser?> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id) ?? null;
            if (user == null) return null;
            user.IsActive = false;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
