using picpay_challenge.DTOs.UserDTOs;
using picpay_challenge.Repositories;
using PicPayChallenge.Models;
using System.Text.RegularExpressions;
namespace PicPayChallenge.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public BaseUser CreateUser(CreateUserPayloadDTO UserPayload)
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
                StoreName = isStorekeeper? UserPayload.StoreName : null,
                UserType = isStorekeeper ? BaseUser.UserTypes.Storekeeper : BaseUser.UserTypes.User

            };
            return _userRepository.Create(payload);
        }
        public BaseUser GetById(int id)
        {
            return _userRepository.GetById(id);
        }
        public bool Delete(int id)
        {
            return _userRepository.Delete(id);
        }
    }
}
