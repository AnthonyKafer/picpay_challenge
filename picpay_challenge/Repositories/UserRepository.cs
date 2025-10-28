using picpay_challenge.DTOs.UserDTOs;
using PicPayChallenge;
using PicPayChallenge.Models;
using System.Threading.Tasks;

namespace picpay_challenge.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public BaseUser Create(BaseUser userPayload)
        {
            _context.Users.Add(userPayload);
            _context.SaveChanges();
            return userPayload;
        }

        public bool Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id) ?? null;
            if (user == null) return false;
            user.IsActive = false;
            _context.SaveChanges();
            return true;

        }
        public BaseUser GetById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id) ?? null;
        }
    }
}
