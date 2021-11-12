using Furion.FriendlyException;
using Heavens.Core.Extension.PageQueayFilter.common;
using System.Linq.Expressions;
using System.Reflection;

namespace Heavens.Core.Extension.PageQueayFilter;

/// <summary>
/// IQueryable 扩展
/// </summary>
public static partial class QueryableExtention
{
    /// <summary>
    /// 多字段排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="orderConditions"></param>
    /// <returns></returns>
    public static IQueryable<T> OrderConditions<T>(this IQueryable<T> query, List<ListSortDirection> orderConditions)
    {
        if (orderConditions == null || !orderConditions.Any())
        {
            return query;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "o");
        for (int i = 0; i < orderConditions.Count; i++)
        {
            ListSortDirection orderinfo = orderConditions[i];
            string fieldName = orderinfo.FieldName;

            Type t = typeof(T);
            PropertyInfo property = t.GetProperty(fieldName) ?? t.GetProperty(fieldName.ToUpperFirstLetter());
            if (property == null)
            {
                throw Oops.Oh(Excode.FIELD_IN_TYPE_NOT_FOUND, fieldName, t.Name);
            }
            //创建一个访问属性的表达式
            MemberExpression propertyAccess = Expression.MakeMemberAccess(parameter, property);
            LambdaExpression orderByExp = Expression.Lambda(propertyAccess, parameter);
            string OrderName = i > 0 ? "ThenBy" : "OrderBy";
            OrderName = OrderName + (orderinfo.SortType.Equals(ListSortType.Desc) ? "Descending" : "");
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
            query = query.Provider.CreateQuery<T>(resultExp);
        }
        return query;
    }
}
