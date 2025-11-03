using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Repositories.Interfaces;
using picpay_challenge.Helpers;
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
        public List<ResponseUserDTO>? FindMany(UserQuery query)
        {
            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FullName))
            {
                users = users.Where(user => user.FullName.Contains(query.FullName));
            }
            if (!string.IsNullOrWhiteSpace(query.CPF))
            {
                users = users.Where(user => user.CPF.Contains(query.CPF));
            }
            if (!string.IsNullOrWhiteSpace(query.CNPJ))
            {
                users = users.Where(user => user.CNPJ.Contains(query.CNPJ));
            }
            if (query.Id != null)
            {
                users = users.Where(user => user.Id == query.Id);
            }
            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                users = users.Where(user => user.Email.Contains(query.Email));
            }
            if (query.Role != null)
            {
                users = users.Where(user => user.Role == query.Role);
            }

            if (query.CreatedAt != null)
            {
                users = users.Where(user => user.CreateAt >= query.CreatedAt);
            }
            if (query.UpdatedAt != null)
            {
                users = users.Where(user => user.UpdatedAt >= query.UpdatedAt);
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    users = query.IsDescending ? users.OrderByDescending(x => x.CreateAt) : users.OrderBy(x => x.CreateAt);
                }
                else if (query.SortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    users = query.IsDescending ? users.OrderByDescending(x => x.UpdatedAt) : users.OrderBy(x => x.UpdatedAt);
                }
                else if (query.SortBy.Equals("FullName", StringComparison.OrdinalIgnoreCase))
                {
                    users = query.IsDescending ? users.OrderByDescending(x => x.FullName) : users.OrderBy(x => x.FullName);
                }

                else users = query.IsDescending ? users.OrderByDescending(x => x.Id) : users.OrderBy(x => x.Id);
            }

            users = users.Where(user => user.IsActive == query.IsActive);

            return users.Skip((query.CurrentPage - 1) * query.PageCount).Take(query.PageCount).Select(user =>
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
            var userCredentials = _context.Users.FirstOrDefault(x => x.Email == Email);
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
