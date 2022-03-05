using Heavens.Core.Extension.QueayFilter.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.QueayFilter.common;
public class Request
{
    /// <summary>
    /// 数据限制
    /// </summary>
    public int Limit { get; set; }
    /// <summary>
    /// 排序集合
    /// </summary>
    public SortBy Sort { get; set; } = new SortBy() { Field = "Id" };

    /// <summary>
    /// 查询条件组
    /// </summary>
    public ICollection<FilterRule> Filters { get; set; } = new List<FilterRule>();

    /// <summary>
    /// 获取查询条件表达式树
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Expression<Func<T, bool>> GetRulesExpression<T>(List<IQueryAction<T>>? queryActions = null)
    {
        return FilterHelper.GetExpression(Filters, queryActions);
    }
}
