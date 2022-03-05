using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.QueayFilter.common;

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
/// <typeparam name="T">实体类型</typeparam>
/// <typeparam name="TSortResult">排序类型</typeparam>
/// <typeparam name="TValueResult">值比对类型</typeparam>
public class QueryAction<T, TSortResult, TValueResult> : IQueryAction<T>
{
    public QueryAction(string field, Expression<Func<T, TSortResult>> sortKeySelector, Expression<Func<T, TValueResult>> filterFunc)
    {
        Field = field;
        SortExp = sortKeySelector;
        FilterExp = filterFunc;
    }

    public string Field { get; set; }

    public LambdaExpression SortExp { get; set; }
    public LambdaExpression FilterExp { get; set; }
}
