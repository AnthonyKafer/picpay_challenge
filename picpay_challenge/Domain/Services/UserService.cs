using AutoMapper;
using picpay_challenge.Domain.DTOs;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Mappers.UserMappers;
using picpay_challenge.Domain.Models.User;
using picpay_challenge.Domain.Repositories;
using picpay_challenge.Domain.Services.Interfaces;
using picpay_challenge.Helpers;
using System.Net;
using System.Threading.Tasks;

namespace picpay_challenge.Domain.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(UserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<BaseUser> ValidadateUserCredentials(LoginUserDTO payload)
        {
            var user = await _userRepository.GetUserCredentials(payload.Email);
            if (user == null) throw new HttpException(HttpStatusCode.NotFound, "Account not found");
            if (user.Password != Encriptor.Encrypt(payload.Password)) throw new HttpException(HttpStatusCode.Unauthorized, "User or Password incorrect");
            return user;
        }

        public async Task<ResponseSingleUserDTO> Create(CreateUserDTO UserPayload)
        {

            bool isStorekeeper = UserPayload.CNPJ != null && UserPayload.StoreName != null;
            var payload = new BaseUser
            {
                FullName = UserPayload.FullName,
                Email = UserPayload.Email,
                Password = Encriptor.Encrypt(UserPayload.Password),
                CPF = Sanitizer.OnlyDigits(UserPayload.CPF),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsActive = true,
                Balance = UserPayload.Balance,
                CNPJ = isStorekeeper ? Sanitizer.OnlyDigits(UserPayload.CNPJ) : null,
                StoreName = isStorekeeper ? UserPayload.StoreName : null,
                Role = isStorekeeper ? BaseUser.Roles.Storekeeper : BaseUser.Roles.User

            };
            var user = await _userRepository.AddAsync(payload);
            return _mapper.Map<ResponseSingleUserDTO>(user);
        }
        public async Task<ResponseSingleUserDTO> CreateAdmin(CreateUserDTO UserPayload)
        {
            var payload = new BaseUser
            {
                FullName = UserPayload.FullName,
                Email = UserPayload.Email,
                Password = Encriptor.Encrypt(UserPayload.Password),
                CPF = Sanitizer.OnlyDigits(UserPayload.CPF),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsActive = true,
                Balance = 0,
                CNPJ = null,
                StoreName = null,
                Role = BaseUser.Roles.Admin
            };
            var user = await _userRepository.AddAsync(payload);
            return _mapper.Map<ResponseSingleUserDTO>(user);
        }

        public async Task<ResponseListUserDTO> FindMany(UserQuery query)
        {
            var users = await _userRepository.GetAllAsync(query);
            return _mapper.Map<ResponseListUserDTO>(users);
        }
        public async Task<ResponseSingleUserDTO> FindById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<ResponseSingleUserDTO>(user);
        }
        public async Task<ResponseSingleUserDTO> Delete(int id)
        {
            var res = await _userRepository.Delete(id);
            return _mapper.Map<ResponseSingleUserDTO>(id);

        }
    }
}
