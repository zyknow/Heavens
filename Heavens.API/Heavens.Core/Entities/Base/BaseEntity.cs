using Furion.DatabaseAccessor;
using Heavens.Core.Authorizations;

namespace Heavens.Core.Entities.Base;

public abstract class BaseEntity<TKey> : CommonEntity<TKey>, IBaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 根据httpToken设置更新字段,如果没有token信息则只设置UpdatedTime
    /// </summary>
    public void SetUpdateByHttpToken()
    {

        if (!TokenInfo.Account.IsEmpty())
        {
            UpdatedBy = TokenInfo.Account;
        }

        if (TokenInfo.Id > 0)
        {
            UpdatedId = TokenInfo.Id;
        }

        UpdatedTime = DateTime.Now;
    }
    /// <summary>
    /// 根据httpToken设置创建字段,如果没有token信息则只设置CreatedTime
    /// </summary>
    public void SetCreateByHttpToken()
    {
        if (!TokenInfo.Account.IsEmpty())
        {
            CreatedBy = TokenInfo.Account;
        }

        if (TokenInfo.Id > 0)
        {
            CreatedId = TokenInfo.Id;
        }

        CreatedTime = DateTime.Now;
    }
}

public abstract class BaseEntity : BaseEntity<int>, IBaseEntity
{

}

public abstract class CommonEntity : CommonEntity<int, MasterDbContextLocator>
{
}

public abstract class CommonEntity<TKey> : CommonEntity<TKey, MasterDbContextLocator>
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1, TDbContextLocator2> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
    where TDbContextLocator2 : class, IDbContextLocator
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1, TDbContextLocator2, TDbContextLocator3> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
    where TDbContextLocator2 : class, IDbContextLocator
    where TDbContextLocator3 : class, IDbContextLocator
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1, TDbContextLocator2, TDbContextLocator3, TDbContextLocator4> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
    where TDbContextLocator2 : class, IDbContextLocator
    where TDbContextLocator3 : class, IDbContextLocator
    where TDbContextLocator4 : class, IDbContextLocator
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1, TDbContextLocator2, TDbContextLocator3, TDbContextLocator4, TDbContextLocator5> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
    where TDbContextLocator2 : class, IDbContextLocator
    where TDbContextLocator3 : class, IDbContextLocator
    where TDbContextLocator4 : class, IDbContextLocator
    where TDbContextLocator5 : class, IDbContextLocator
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1, TDbContextLocator2, TDbContextLocator3, TDbContextLocator4, TDbContextLocator5, TDbContextLocator6> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
    where TDbContextLocator2 : class, IDbContextLocator
    where TDbContextLocator3 : class, IDbContextLocator
    where TDbContextLocator4 : class, IDbContextLocator
    where TDbContextLocator5 : class, IDbContextLocator
    where TDbContextLocator6 : class, IDbContextLocator
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1, TDbContextLocator2, TDbContextLocator3, TDbContextLocator4, TDbContextLocator5, TDbContextLocator6, TDbContextLocator7> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
    where TDbContextLocator2 : class, IDbContextLocator
    where TDbContextLocator3 : class, IDbContextLocator
    where TDbContextLocator4 : class, IDbContextLocator
    where TDbContextLocator5 : class, IDbContextLocator
    where TDbContextLocator6 : class, IDbContextLocator
    where TDbContextLocator7 : class, IDbContextLocator
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1, TDbContextLocator2, TDbContextLocator3, TDbContextLocator4, TDbContextLocator5, TDbContextLocator6, TDbContextLocator7, TDbContextLocator8> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
    where TDbContextLocator2 : class, IDbContextLocator
    where TDbContextLocator3 : class, IDbContextLocator
    where TDbContextLocator4 : class, IDbContextLocator
    where TDbContextLocator5 : class, IDbContextLocator
    where TDbContextLocator6 : class, IDbContextLocator
    where TDbContextLocator7 : class, IDbContextLocator
    where TDbContextLocator8 : class, IDbContextLocator
{
}

public abstract class PrivateCommonEntity<TKey> : BaseEntityProp<TKey>, IPrivateEntity
{
}

public class BaseEntityProp<TKey>
{
    /// <summary>
    /// id主键
    /// </summary>
    public TKey Id { get; set; }
    /// <summary>
    /// 是否已被删除
    /// </summary>
    public bool IsDeleted { get; set; } = false;
    /// <summary>
    /// 创建者id
    /// </summary>
    public int CreatedId { get; set; }
    /// <summary>
    /// 创建者
    /// </summary>
    public string CreatedBy { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    /// <summary>
    /// 更新者id
    /// </summary>
    public int UpdatedId { get; set; }
    /// <summary>
    /// 更新者
    /// </summary>
    public string UpdatedBy { get; set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}

public class BaseEntityProp : BaseEntityProp<int>
{

}