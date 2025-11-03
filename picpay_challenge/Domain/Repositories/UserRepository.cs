using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Repositories.Interfaces;
using System.Net;

namespace picpay_challenge.Domain.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public List<ResponseUserDTO>? FindMany()
        {
            List<ResponseUserDTO>? users = _context.Users.Select(user =>
             new ResponseUserDTO
             {
                 Id = user.Id,
                 FullName = user.FullName,
                 Email = user.Email,
                 Role = user.Role,
                 CPF = user.CPF,
                 Balance = user.Balance,
                 CNPJ = user.CNPJ,
                 StoreName = user.StoreName,
             }
            ).ToList();
            return users ?? null;
        }
        public ResponseUserDTO? FindById(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id) ?? null;
            return new ResponseUserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CPF = user.CPF,
                Balance = user.Balance,
                CNPJ = user.CNPJ,
                StoreName = user.StoreName,
            };
        }

        public ResponseUserDTO? Update(int Id, UpdateUserDTO payload)
        {

            BaseUser user = _context.Users.FirstOrDefault(x => x.Id == Id)!;
            user.FullName = payload.FullName == null ? user.FullName : payload.FullName;
            user.Email = payload.Email == null ? user.Email : payload.Email;
            user.Password = payload.Password == null ? user.Password : payload.Password;
            user.CPF = payload.CPF == null ? user.CPF : payload.CPF;
            user.CNPJ = payload.CNPJ == null ? user.CNPJ : payload.CNPJ;
            user.StoreName = payload.StoreName == null ? user.StoreName : payload.StoreName;

            _context.Users.Update(user);
            return new ResponseUserDTO()
            {
                Id = user.Id,
                FullName = user.FullName,
                Role = user.Role,
                Email = user.Email,
                CPF = user.CPF,
                Balance = user.Balance,
                CNPJ = user.CNPJ,
                StoreName = user.StoreName,
            };
        }

        public ResponseUserDTO Create(BaseUser userPayload)
        {
            try
            {
                _context.Users.Add(userPayload);
                _context.SaveChanges();
                return new ResponseUserDTO()
                {
                    Id = userPayload.Id,
                    FullName = userPayload.FullName,
                    Email = userPayload.Email,
                    Role = userPayload.Role,
                    CPF = userPayload.CPF,
                    Balance = userPayload.Balance,
                    CNPJ = userPayload.CNPJ,
                    StoreName = userPayload.StoreName,
                };
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
        public BaseUser? GetUserCredentials(string Email)
        {
            var userCredentials = _context.Users.SingleOrDefault(x => x.Email == Email);
            return userCredentials ?? null;
        }
        public ResponseUserDTO? Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id) ?? null;
            if (user == null) return null;
            user.IsActive = false;
            _context.SaveChanges();
            return new ResponseUserDTO()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CPF = user.CPF,
                Balance = user.Balance,
                CNPJ = user.CNPJ,
                StoreName = user.StoreName,
            };
        }
    }
}
