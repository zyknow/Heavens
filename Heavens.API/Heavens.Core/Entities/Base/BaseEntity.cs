using Furion.DatabaseAccessor;
using Heavens.Core.Authorizations;
using System;

namespace Heavens.Core.Entities.Base;

public abstract class BaseEntity<TKey> : CommonEntity<TKey>, IBaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
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

        UpdatedTime = DateTimeOffset.Now;
    }
    
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

        CreatedTime = DateTimeOffset.Now;
    }
}

public abstract class BaseEntity : BaseEntity<int>, IBaseEntity
{

}


public abstract class CommonEntity<TKey> : CommonEntity<TKey, MasterDbContextLocator>
{
}

public abstract class CommonEntity<TKey, TDbContextLocator1> : PrivateCommonEntity<TKey>
    where TDbContextLocator1 : class, IDbContextLocator
{
}


public abstract class PrivateCommonEntity<TKey> : BaseEntityProp<TKey>, IPrivateEntity
{
}
