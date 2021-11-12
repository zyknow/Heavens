using Furion.DatabaseAccessor;
using Heavens.Core.Authorizations;
using Heavens.Core.Entities.Base;

namespace Heavens.Core.Extentions;

/// <summary>
/// furion提供的数据库操作扩展
/// </summary>
public static partial class RepositoryExtention
{
    /// <summary>
    /// 根据实体假删除，并更新Update信息字段
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="repository"></param>
    /// <param name="entity"></param>
    /// <returns>Entity</returns>
    public static async Task<TEntity> FakeDeleteSetInfoNowAsync<TEntity>(this IPrivateRepository<TEntity> repository, TEntity entity)
        where TEntity : class, IBaseEntity, IPrivateEntity, new()
    {
        entity.IsDeleted = true;
        entity.SetUpdateByHttpToken();
        return (await repository.UpdateIncludeNowAsync(entity, new string[] { "IsDeleted", "UpdatedId", "UpdatedBy", "UpdatedTime" })).Entity;
    }
    /// <summary>
    /// 根据Id假删除，并更新Update信息字段
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">实体主键类型</typeparam>
    /// <param name="repository"></param>
    /// <param name="id"></param>
    /// <returns>Id</returns>
    public static async Task<TKey> FakeDeleteSetInfoNowAsync<TEntity, TKey>(this IPrivateRepository<TEntity> repository, TKey id)
        where TEntity : class, IBaseEntity<TKey>, IPrivateEntity, new()
    {
        TEntity delEntity = new TEntity()
        {
            Id = id,
            IsDeleted = true
        };
        delEntity.SetUpdateByHttpToken();
        return (await repository.UpdateIncludeNowAsync(delEntity, new string[] { "IsDeleted", "UpdatedId", "UpdatedBy", "UpdatedTime" })).Entity.Id;
    }
    /// <summary>
    /// 根据Id批量假删除，并更新Update信息字段
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">实体主键类型</typeparam>
    /// <param name="repository"></param>
    /// <param name="ids"></param>
    /// <returns>操作数量</returns>
    public static Task<int> FakeDeleteSetInfoNowAsync<TEntity, TKey>(this IPrivateRepository<TEntity> repository, IEnumerable<TKey> ids)
        where TEntity : class, IBaseEntity<TKey>, IPrivateEntity, new()
    {
        return repository.Context.BatchUpdate<TEntity>()
                .Where(t => ids.Contains(t.Id))
                .Set(t => t.IsDeleted, t => true)
                .Set(t => t.UpdatedBy, t => TokenInfo.Account)
                .Set(t => t.UpdatedId, t => TokenInfo.Id)
                .Set(t => t.UpdatedTime, t => DateTime.Now)
                .ExecuteAsync();
    }
}
