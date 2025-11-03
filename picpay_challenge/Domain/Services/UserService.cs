using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Repositories;
using picpay_challenge.Domain.Services.Interfaces;
using picpay_challenge.Helpers;
using System.Net;

namespace picpay_challenge.Domain.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public BaseUser ValidadateUserCredentials(LoginUserDTO payload)
        {
            var user = _userRepository.GetUserCredentials(payload.Email);
            if (user == null) throw new HttpException(HttpStatusCode.NotFound, "User does not exist");
            if (user.Password != Encriptor.Encrypt(payload.Password)) throw new HttpException(HttpStatusCode.Unauthorized, "User or Password incorrect");
            return user;
        }

        public ResponseUserDTO Create(CreateUserDTO UserPayload)
        {

            bool isStorekeeper = UserPayload.CNPJ != null && UserPayload.StoreName != null;
            var payload = new BaseUser
            {
                FullName = UserPayload.FullName,
                Email = UserPayload.Email,
                Password = Encriptor.Encrypt(UserPayload.Password),
                CPF = Sanitizer.OnlyDigits(UserPayload.CPF),
                CreateAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsActive = true,
                Balance = UserPayload.Balance,
                CNPJ = isStorekeeper ? Sanitizer.OnlyDigits(UserPayload.CNPJ) : null,
                StoreName = isStorekeeper ? UserPayload.StoreName : null,
                Role = isStorekeeper ? BaseUser.Roles.Storekeeper : BaseUser.Roles.User

            };
            return _userRepository.Create(payload);
        }
        public ResponseUserDTO CreateAdmin(CreateUserDTO UserPayload)
        {
            var payload = new BaseUser
            {
                FullName = UserPayload.FullName,
                Email = UserPayload.Email,
                Password = Encriptor.Encrypt(UserPayload.Password),
                CPF = Sanitizer.OnlyDigits(UserPayload.CPF),
                CreateAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsActive = true,
                Balance = 0,
                CNPJ = null,
                StoreName = null,
                Role = BaseUser.Roles.Admin
            };
            return _userRepository.Create(payload);
        }


        public List<ResponseUserDTO>? FindMany(UserQuery query)
        {
            return _userRepository.FindMany(query);
        }
        public ResponseUserDTO? FindById(int id)
        {
            return _userRepository.FindById(id);
        }
        public BaseUser? FindByEmail(string email)
        {
            return _userRepository.GetUserCredentials(email);
        }
        public ResponseUserDTO? Delete(int id)
        {
            return _userRepository.Delete(id);
        }
    }
}
