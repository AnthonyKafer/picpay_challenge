using picpay_challenge.DTOs.UserDTOs;
using picpay_challenge.Repositories;
using PicPayChallenge.Models;
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
                CNPJ = UserPayload.CNPJ ?? null,
                StoreName = UserPayload.StoreName ?? null
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

