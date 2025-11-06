using Microsoft.EntityFrameworkCore.Storage;
using picpay_challenge.Domain.DTOs;
using picpay_challenge.Domain.Models;
using picpay_challenge.Helpers;
using System.Data;
namespace UDS.GrupoCard.Package.Startup.ApiLayers.Interfaces;

public interface IRepositoryBase
    <TEntity, TFilter>
    where TEntity : EntityBase
    where TFilter : BaseQuery
{
    public Task<TEntity?> GetByIdAsync(int id);

    public Task<BaseResponse<TEntity>> GetAllAsync(TFilter filter);

    public Task<TEntity> AddAsync(TEntity add);

    public Task<TEntity> UpdateAsync(TEntity entity);

    public Task SaveAsync();
}