using Furion.UnifyResult;
using Furion;
using Heavens.Core.Extension.QueayFilter.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Heavens.Core.Extension.Extensions;

namespace Heavens.Core.Extension.QueayFilter.common;
public abstract class BaseRequest
{
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
    public virtual Expression<Func<T, bool>> GetRulesExpression<T>(List<IQueryAction<T>>? queryActions = null)
    {
        var exp = FilterHelper.GetExpression(Filters, queryActions);

        // 开发环境下填入过滤条件
        if (App.HostEnvironment.IsDevelopment())
            UnifyContext.Fill(@$"{exp.ToLambdaString()}");

        return exp;
    }
}
