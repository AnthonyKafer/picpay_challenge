using System.Text.RegularExpressions;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Repositories;
using picpay_challenge.Domain.Services.Interfaces;
using picpay_challenge.Domain.Models;

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
            if (user == null) return false;
            if (user.Password != payload.Password) return false;
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
                CPF = UserPayload.CPF,
                CreateAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsActive = true,
                Balance = UserPayload.Balance,
                CNPJ = isStorekeeper ? UserPayload.CNPJ : null,
                StoreName = isStorekeeper ? UserPayload.StoreName : null,
                UserType = isStorekeeper ? BaseUser.UserTypes.Storekeeper : BaseUser.UserTypes.User

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
