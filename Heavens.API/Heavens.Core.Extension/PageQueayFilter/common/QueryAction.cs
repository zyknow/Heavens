using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.PageQueayFilter.common;

/// <summary>
/// PageRequest增强查询过滤
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IQueryAction<T>
{
    public string Field { get; set; }

    public LambdaExpression SortExp { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public LambdaExpression FilterExp { get; set; }
}

/// <summary>
/// PageRequest增强查询过滤
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TKey"></typeparam>
public class QueryAction<T, TKey> : IQueryAction<T>
{
    public QueryAction(string field, Expression<Func<T, TKey>> sortKeySelector, Expression<Func<T, TKey>> filterFunc)
    {
        Field = field;
        SortExp = sortKeySelector;
        FilterExp = filterFunc;
    }

    public string Field { get; set; }

    public LambdaExpression SortExp { get; set; }
    public LambdaExpression FilterExp { get; set; }
}
