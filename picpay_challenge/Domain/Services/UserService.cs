using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Repositories;
using picpay_challenge.Domain.Services.Interfaces;
using picpay_challenge.Helpers;
using System.Net;
using System.Text.RegularExpressions;

namespace picpay_challenge.Domain.Services
{
    public class UserService : IServiceInterface<BaseUser>
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool ValidadateUserCredentials(LoginUserDTO payload)
        {
            var user = _userRepository.GetUserCredentials(payload.Email);
            if (user == null) throw new HttpException(HttpStatusCode.NotFound, "User does not exist");
            if (user.Password != payload.Password) throw new HttpException(HttpStatusCode.Unauthorized, "User or Password incorrect");
            return true;
        }

        public BaseUser Create(CreateUserDTO UserPayload)
        {

            bool isStorekeeper = UserPayload.CNPJ != null && UserPayload.StoreName != null;

            var payload = new BaseUser
            {
                FullName = UserPayload.FullName,
                Email = UserPayload.Email,
                Password = UserPayload.Password,
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
        public void ChangeUserBalance(int Id, decimal value)
        {
            _userRepository.ChangeUserBalance(Id, value);
            return;
        }
        public List<BaseUser>? FindMany()
        {
            return _userRepository.FindMany();
        }
        public BaseUser? FindById(int id)
        {
            return _userRepository.FindById(id);
        }
        public BaseUser? Delete(int id)
        {
            return _userRepository.Delete(id);
        }
    }
}
