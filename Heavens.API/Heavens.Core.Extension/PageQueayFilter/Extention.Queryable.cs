using Heavens.Core.Extension.PageQueayFilter.common;
using Heavens.Core.Extension.PageQueayFilter.helper;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Heavens.Core.Extension.PageQueayFilter;

/// <summary>
/// IQueryable 扩展
/// </summary>
public static partial class QueryableExtention
{

    public static IQueryable<T> SortBy<T>(this IQueryable<T> query, SortBy sort, List<IQueryAction<T>>? actions = null)
    {
        var action = actions?.FirstOrDefault(f => f.Field.ToUpperFirstLetter() == sort.Field.ToUpperFirstLetter() && f.SortExp != null);
        if (action == null)
            return QueryableHelper.OrderCondition(query, sort);
        return QueryableHelper.OrderCondition(query, sort, action.SortExp);
    }
}
