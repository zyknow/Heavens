using Heavens.Core.Entities.Base;
using Heavens.Core.Extension.PageQueayFilter;
using Heavens.Core.Extension.PageQueayFilter.common;
using Heavens.Core.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace Heavens.Application._Base;

/// <summary>
/// 继承此类即可实现基础方法
/// 方法包括：CURD
/// </summary>
/// <typeparam name="TKey">数据实体主键类型</typeparam>
/// <typeparam name="TEntity">数据实体类型</typeparam>
/// <typeparam name="TEntityDto">数据实体Dto类型</typeparam>
[ApiDescriptionSettings(Order = 0)]
[Authorize]
public abstract class BaseAppService<TKey, TEntity, TEntityDto> : IDynamicApiController
    where TEntity : class, IBaseEntity<TKey>, IPrivateEntity, new()
    where TEntityDto : class, new()
{
    protected IRepository<TEntity> _repository { get; }

    /// <summary>
    /// 继承此类即可实现基础方法
    /// 方法包括：CURD、获取全部、分页获取 
    /// </summary>
    /// <param name="repository"></param>
    protected BaseAppService(IRepository<TEntity> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取分页
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public Task<PagedList<TEntityDto>> Page(PageRequest request)
    {
        System.Linq.Expressions.Expression<System.Func<TEntity, bool>> expression = request.GetRulesExpression<TEntity>();

        return _repository
            .Where(expression)
            .OrderConditions(request.OrderConditions)
            .Select(x => x.Adapt<TEntityDto>())
            .ToPagedListAsync();
    }

    /// <summary>
    /// 根据Id查询
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual Task<TEntityDto> GetById([Required] TKey id)
    {
        return _repository.Where(r => r.Id.Equals(id)).Select(u => u.Adapt<TEntityDto>()).FirstOrDefaultAsync();
    }

    /// <summary>
    /// 获取所有
    /// </summary>
    /// <returns></returns>
    public virtual Task<List<TEntityDto>> GetAll()
    {
        return _repository.AsQueryable().Select(u => u.Adapt<TEntityDto>()).ToListAsync();
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="modelDto"></param>
    /// <returns></returns>
    public virtual async Task<TEntityDto> Add(TEntityDto modelDto)
    {
        TEntity model = modelDto.Adapt<TEntity>();
        model.SetCreateByHttpToken();
        return (await _repository.InsertNowAsync(model)).Entity.Adapt<TEntityDto>();
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="modelDto"></param>
    /// <returns></returns>
    public virtual async Task<TEntityDto> Update(TEntityDto modelDto)
    {
        TEntity model = modelDto.Adapt<TEntity>();
        model.SetUpdateByHttpToken();
        return (await _repository.UpdateNowAsync(model)).Entity.Adapt<TEntityDto>();
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns>被删除的实体主键</returns>
    [HttpDelete]
    public virtual Task<TKey> DeleteById([Required] TKey id)
    {
        return _repository.FakeDeleteSetInfoNowAsync(id);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns>删除数量</returns>
    [HttpDelete]
    public virtual Task<int> DeleteByIds([FromBody] TKey[] ids)
    {
        if (ids.IsEmpty())
        {
            throw Oops.Oh(Excode.REQUEST_DATA_EMPTY);
        }

        return _repository.FakeDeleteSetInfoNowAsync(ids.Select(id => id));
    }
}


/// <summary>
/// 继承此类即可实现基础方法
/// 方法包括：CURD
/// </summary>
/// <typeparam name="TEntity">数据实体类型</typeparam>
/// <typeparam name="TEntityDto">数据实体类型</typeparam>
public abstract class BaseAppService<TEntity, TEntityDto> : BaseAppService<int, TEntity, TEntityDto>
    where TEntity : class, IBaseEntity, IPrivateEntity, new()
    where TEntityDto : class, new()
{
    protected BaseAppService(IRepository<TEntity> repository) : base(repository)
    {
    }
}

/// <summary>
/// 继承此类即可实现基础方法
/// 方法包括：CURD
/// </summary>
/// <typeparam name="TEntity">数据实体类型</typeparam>
public abstract class BaseAppService<TEntity> : BaseAppService<int, TEntity, TEntity>
    where TEntity : class, IBaseEntity, IPrivateEntity, new()
{
    protected BaseAppService(IRepository<TEntity> repository) : base(repository)
    {
    }
}
