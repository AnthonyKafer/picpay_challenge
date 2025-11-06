using Microsoft.EntityFrameworkCore;
using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.DTOs;
using picpay_challenge.Domain.Models;
using picpay_challenge.Helpers;
using System.Linq.Expressions;
using System.Reflection;
using UDS.GrupoCard.Package.Startup.ApiLayers.Interfaces;

public class RepositoryBase<TEntity, TFilter>
    : IRepositoryBase<TEntity, TFilter>
    where TEntity : EntityBase
    where TFilter : BaseQuery
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public RepositoryBase(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveAsync();
        return entity;
    }
    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        var res = await _dbSet.FindAsync(id);
        return res;
    }

    public virtual async Task<BaseResponse<TEntity>> GetAllAsync(TFilter filter)
    {
        IQueryable<TEntity> query = _dbSet;
        int total = (int)Math.Ceiling(Convert.ToDecimal(query.Count()) / Convert.ToDecimal(filter.PageCount));

        ApplyOrdering(query, filter);

        List<TEntity> data = await ApplyFilter(query, filter)
            .Skip((filter.CurrentPage - 1) * filter.PageCount)
            .Take(filter.PageCount)
            .ToListAsync();
        return new BaseResponse<TEntity>()
        {
            Data = data,
            CurrentPage = filter.CurrentPage,
            TotalPages = total == 0 ? 1 : total,
        };
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await SaveAsync();
        return entity;
    }

    public virtual async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, TFilter filter)
    {
        if (filter.Id.HasValue)
            query = query.Where(e => e.Id == filter.Id.Value);


        if (filter.CreatedAt.HasValue)
            query = query.Where(e => e.CreatedAt >= filter.CreatedAt.Value);
        if (filter.UpdatedAt.HasValue)
            query = query.Where(e => e.UpdatedAt >= filter.UpdatedAt.Value);


        query = ApplyOrdering(query, filter);

        return query;
    }


    protected virtual IQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> query, TFilter filter)
    {
        if (string.IsNullOrWhiteSpace(filter.SortBy))
            return query;

        var property = typeof(TEntity).GetProperty(filter.SortBy,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var propertyAccess = Expression.Property(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);
        Console.WriteLine(filter.IsDescending + "kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk");
        string methodName = filter.IsDescending ? "OrderByDescending" : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(TEntity), property.PropertyType },
            query.Expression,
            Expression.Quote(orderByExpression));

        return query.Provider.CreateQuery<TEntity>(resultExpression);
    }
}
