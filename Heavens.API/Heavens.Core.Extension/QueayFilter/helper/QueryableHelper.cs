using Furion.FriendlyException;
using Heavens.Core.Extension.QueayFilter.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.QueayFilter.helper;

internal class QueryableHelper
{
    /// <summary>
    /// 获取排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="sort"></param>
    /// <returns></returns>
    internal static IQueryable<T> OrderCondition<T>(IQueryable<T> query, SortBy sort, LambdaExpression? exp = null)
    {
        if (sort == null || sort.Field.IsEmpty())
        {
            return query;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "o");

        SortBy orderinfo = sort;
        string fieldName = orderinfo.Field;

        Type t = typeof(T);

        Type? propertyType = exp?.Body.Type;
        if (exp == null)
        {
            var property = t.GetProperty(fieldName) ?? t.GetProperty(fieldName.ToUpperFirstLetter());
            propertyType = property?.PropertyType;
            if (propertyType == null)
                throw Oops.Oh(Excode.FIELD_IN_TYPE_NOT_FOUND, fieldName, t.Name);

            //创建一个访问属性的表达式
            MemberExpression propertyAccess = Expression.MakeMemberAccess(parameter, property!);
            exp = Expression.Lambda(propertyAccess, parameter);
        }

        string OrderName = "OrderBy";
        OrderName += (orderinfo.SortType.Equals(SortType.Desc) ? "Descending" : "");
        MethodCallExpression resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(T), propertyType! }, query.Expression, Expression.Quote(exp));
        query = query.Provider.CreateQuery<T>(resultExp);
        return query;
    }
}
