using AutoMapper;
using picpay_challenge.Domain.DTOs;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Models.User;

namespace picpay_challenge.Domain.Mappers.UserMappers
{
    public class SingleUserMapper : Profile
    {
        public SingleUserMapper()
        {
            CreateMap<BaseUser, ResponseSingleUserDTO>();
        }

    }
    public class ListUserMapper : Profile
    {
        public ListUserMapper()
        {
            CreateMap<BaseResponse<BaseUser>, ResponseListUserDTO>();
        }
    }

}
