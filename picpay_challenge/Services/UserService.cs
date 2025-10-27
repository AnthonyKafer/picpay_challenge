using PicPayChallenge;
namespace PicPayChallenge.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

    }
}