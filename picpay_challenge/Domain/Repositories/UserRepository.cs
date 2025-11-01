using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Repositories.Interfaces;
using System.Net;

namespace picpay_challenge.Domain.Repositories
{
    public class UserRepository : IRepositoryInterface<BaseUser>
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public List<BaseUser>? FindMany()
        {
            List<BaseUser>? users = _context.Users.ToList() ?? null;
            return users ?? null;
        }
        public BaseUser? FindById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id) ?? null;
        }

        public BaseUser? Update(int Id, BaseUser payload)
        {
            return _context.Users.FirstOrDefault();
        }
        public void ChangeUserBalance(int Id, decimal value)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == Id);
            if (user == null) return;
            user.Balance += value;
            _context.SaveChanges();
            return;
        }

        public BaseUser Create(BaseUser userPayload)
        {
            try
            {
                Console.WriteLine(userPayload);
                _context.Users.Add(userPayload);
                _context.SaveChanges();
                return userPayload;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException.Message;
                if (errorMessage.Split(":")[0] == "23505")
                {
                    throw new HttpException(HttpStatusCode.Conflict, "One of the used credentials is already registered");
                }
                else throw new HttpException(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        public BaseUser GetUserCredentials(string Email)
        {
            var userCredentials = _context.Users.SingleOrDefault(x => x.Email == Email);
            return userCredentials ?? null;
        }
        public BaseUser? Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id) ?? null;
            if (user == null) return null;
            user.IsActive = false;
            _context.SaveChanges();
            return user;

        }
    }
}
