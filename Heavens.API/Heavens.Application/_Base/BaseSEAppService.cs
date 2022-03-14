using Furion.DatabaseAccessor;
using Heavens.Core.Entities.Base;
using Heavens.Core.Extension.SearchEngine;
using Microsoft.AspNetCore.Mvc;

namespace Heavens.Application._Base;

#region BaseSE
/// <summary>
/// 基础CURD
/// CURD带搜索引擎CURD
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TEntityDto"></typeparam>
/// <typeparam name="TPageEntity"></typeparam>
public abstract class BaseSEAppService<TKey, TEntity, TEntityDto, TPageEntity> : BaseAppService<TKey, TEntity, TEntityDto, TPageEntity> where TEntity : class, IBaseEntity<TKey>, IPrivateEntity, new()
    where TEntityDto : class, new()
{
    protected ISearchEngine _searchEngine { get; }

    protected string searchIndex;

    protected BaseSEAppService(IRepository<TEntity> repository, ISearchEngine searchEngine, string searchIndexName = null) : base(repository)
    {
        searchIndex = searchIndexName ?? typeof(TEntity).Name.ToCamelCase();
        _searchEngine = searchEngine;
    }

    /// <summary>
    /// 搜索
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<Meilisearch.SearchResult<TEntityDto>> Search(string search)
    {
        Meilisearch.SearchResult<TEntityDto> result = await _searchEngine.Search<TEntityDto>(searchIndex, search);
        return result;
    }

    [HttpPost]
    public async Task<Meilisearch.SearchResult<TEntityDto>> Search(SearchRequest searchRequest)
    {
        Meilisearch.SearchResult<TEntityDto> result = await _searchEngine.Search<TEntityDto>(searchIndex, searchRequest.Search, searchRequest);
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

    public override async Task DeleteById([Required] TKey id)
    {
        await _searchEngine.DeleteDocuments(searchIndex, new string[] { id.ToString() });
        await base.DeleteById(id);
    }
}
public abstract class SE_BaseAppService<TEntity, TEntityDto> : BaseSEAppService<int, TEntity, TEntityDto, TEntityDto>
    where TEntity : class, IBaseEntity<int>, IPrivateEntity, new()
    where TEntityDto : class, new()
{
    protected SE_BaseAppService(IRepository<TEntity> repository, ISearchEngine searchEngine) : base(repository, searchEngine)
    {
    }
}
#endregion



