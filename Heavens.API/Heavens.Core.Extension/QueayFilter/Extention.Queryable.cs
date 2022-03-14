using Heavens.Core.Extension.QueayFilter.common;
using Heavens.Core.Extension.QueayFilter.helper;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Heavens.Core.Extension.QueayFilter;

/// <summary>
/// IQueryable 扩展
/// </summary>
public static partial class QueryableExtention
{

    public static IQueryable<T> SortBy<T>(this IQueryable<T> query, SortBy sort, List<IQueryAction<T>> actions = null)
    {
        var action = actions?.FirstOrDefault(f => f.Field.ToUpperFirstLetter() == sort.Field.ToUpperFirstLetter() && f.SortExp != null);

        return QueryableHelper.OrderCondition(query, sort, action?.SortExp);
    }
}
