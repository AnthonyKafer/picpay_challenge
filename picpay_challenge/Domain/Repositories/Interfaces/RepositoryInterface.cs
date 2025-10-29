using picpay_challenge.DTOs.UserDTOs;

namespace picpay_challenge.Domain.Repositories.Interfaces
{
    public interface IRepositoryInterface<DTO>
    {
        List<DTO>? FindMany();
        DTO? FindById(int Id);
        DTO Create(DTO payload);


    }
}
