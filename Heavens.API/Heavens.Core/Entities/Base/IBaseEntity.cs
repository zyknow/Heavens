using Furion.DatabaseAccessor;

namespace Heavens.Core.Entities.Base;

public interface IBaseEntity<TKey> : IPrivateEntity
{
    /// <summary>
    /// id主键
    /// </summary>
    public TKey Id { get; set; }
    /// <summary>
    /// 创建者id
    /// </summary>
    public int? CreatedId { get; set; }
    /// <summary>
    /// 创建者
    /// </summary>
    public string CreatedBy { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset? CreatedTime { get; set; }
    /// <summary>
    /// 更新者id
    /// </summary>
    public int? UpdatedId { get; set; }
    /// <summary>
    /// 更新者
    /// </summary>
    public string UpdatedBy { get; set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTimeOffset? UpdatedTime { get; set; }
    /// <summary>
    /// 根据httpToken设置创建字段,如果没有token信息则只设置CreatedTime
    /// </summary>
    void SetCreateByHttpToken();
    /// <summary>
    /// 根据httpToken设置更新字段,如果没有token信息则只设置UpdatedTime
    /// </summary>
    void SetUpdateByHttpToken();
}

public interface IBaseEntity : IBaseEntity<int>
{
}
