using Furion.DatabaseAccessor;
using Heavens.Core.Entities.Base;
using Heavens.Core.Extension.SearchEngine;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Heavens.Application._Base;

/// <summary>
/// 基础CURD
/// CURD带搜索引擎CURD
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TEntityDto"></typeparam>
/// <typeparam name="TEntitySE"></typeparam>
public abstract class BaseSEAppService<TKey, TEntity, TEntityDto, TEntitySE> : BaseAppService<TKey, TEntity, TEntityDto>
    where TEntity : class, IBaseEntity<TKey>, IPrivateEntity, new()
    where TEntityDto : class, new()
{
    public ISearchEngine _searchEngine { get; }

    protected string searchIndex => typeof(TEntity).Name.ToCamelCase();

    protected BaseSEAppService(IRepository<TEntity> repository, ISearchEngine searchEngine) : base(repository)
    {
        _searchEngine = searchEngine;
    }

    /// <summary>
    /// 搜索
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<Meilisearch.SearchResult<TEntityDto>> Search(string search)
    {
        Meilisearch.SearchResult<TEntityDto> result = await _searchEngine.Search<TEntityDto>(searchIndex, search);
        return result;
    }

    public override async Task<TEntityDto> Update(TEntityDto modelDto)
    {
        TEntityDto entity = await base.Update(modelDto);
        await _searchEngine.AddOrUpdate(searchIndex, entity.Adapt<TEntityDto>());
        return entity;
    }

    public override async Task<TEntityDto> Add(TEntityDto modelDto)
    {
        TEntityDto entity = await base.Add(modelDto);
        await _searchEngine.AddOrUpdate(searchIndex, entity.Adapt<TEntityDto>());
        return entity;
    }
    public override async Task<int> DeleteByIds([FromBody] TKey[] ids)
    {
        await _searchEngine.DeleteDocuments(searchIndex, ids.Select(id => id.ToString()).ToArray());
        return await base.DeleteByIds(ids);
    }

    public override async Task<TKey> DeleteById([Required] TKey id)
    {
        await _searchEngine.DeleteDocuments(searchIndex, new string[] { id.ToString() });
        return await base.DeleteById(id);
    }
}
public abstract class SE_BaseAppService<TEntity, TEntityDto, TEntitySE> : BaseSEAppService<int, TEntity, TEntityDto, TEntitySE>
    where TEntity : class, IBaseEntity, IPrivateEntity, new()
    where TEntityDto : class, new()
{
    protected SE_BaseAppService(IRepository<TEntity> repository, ISearchEngine searchEngine) : base(repository, searchEngine)
    {
    }
}
public abstract class SE_BaseAppService<TEntity, TEntitySE> : BaseSEAppService<int, TEntity, TEntity, TEntitySE>
    where TEntity : class, IBaseEntity, IPrivateEntity, new()
{
    protected SE_BaseAppService(IRepository<TEntity> repository, ISearchEngine searchEngine) : base(repository, searchEngine)
    {
    }
}
