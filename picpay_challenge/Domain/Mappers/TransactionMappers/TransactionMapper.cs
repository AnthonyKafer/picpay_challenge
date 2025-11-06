using AutoMapper;
using picpay_challenge.Domain.DTOs;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Models.Transaction;
using picpay_challenge.Domain.Models.User;

namespace picpay_challenge.Domain.Mappers.UserMappers
{

    public class SingleTransactionMapper : Profile
    {
        public SingleTransactionMapper()
        {
            CreateMap<Transaction, ResponseSingleTransactionDTO>();
        }

    }
    public class ListTransactionMapper : Profile
    {
        public ListTransactionMapper()
        {
            CreateMap<Transaction, ResponseSingleTransactionDTO>()
                  .ForMember(dest => dest.Payer, opt => opt.MapFrom(src => new PayerAndPayee
                  {
                      Id = src.Payer.Id,
                      FullName = src.Payer.FullName
                  }))
                  .ForMember(dest => dest.Payee, opt => opt.MapFrom(src => new PayerAndPayee
                  {
                      Id = src.Payee.Id,
                      FullName = src.Payee.FullName
                  }));

            CreateMap<BaseResponse<Transaction>, ResponseListTransactionDTO>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));

        }
    }

}
