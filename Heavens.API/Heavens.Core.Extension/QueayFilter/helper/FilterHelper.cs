using AspectCore.DynamicProxy.Parameters;
using Bing.Expressions;
using Bing.Extensions;
using EasyCaching.Core.Internal;
using Furion.FriendlyException;
using Heavens.Core.Extension.Extensions;
using Heavens.Core.Extension.QueayFilter;
using Heavens.Core.Extension.QueayFilter.common;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;

namespace Heavens.Core.Extension.QueayFilter.helper;

/// <summary>
/// 查询表达式辅助操作类
/// </summary>
public static class FilterHelper
{
    #region 字段
    private static Dictionary<FilterOperate, Func<Expression, Expression, Expression>> ExpressionDict { get; set; } =
        new Dictionary<FilterOperate, Func<Expression, Expression, Expression>>
        {
                {
                    FilterOperate.Equal, Expression.Equal
                },
                {
                    FilterOperate.NotEqual, Expression.NotEqual
                },
                {
                    FilterOperate.Less, Expression.LessThan
                },
                {
                    FilterOperate.Greater, Expression.GreaterThan
                },
                {
                    FilterOperate.LessOrEqual, Expression.LessThanOrEqual
                },
                {
                    FilterOperate.GreaterOrEqual, Expression.GreaterThanOrEqual
                },
                {
                    FilterOperate.StartsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“StartsWith”比较方式只支持字符串类型的数据");
                        }
                        return Expression.Call(left,
                            typeof(string).GetMethod("StartsWith", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"名称为“StartsWith”的方法不存在"),
                            right);
                    }
                },
                {
                    FilterOperate.EndsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“EndsWith”比较方式只支持字符串类型的数据");
                        }
                        return Expression.Call(left,
                            typeof(string).GetMethod("EndsWith", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"名称为“EndsWith”的方法不存在"),
                            right);
                    }
                },
                {
                    FilterOperate.Contains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“Contains”比较方式只支持字符串类型的数据");
                        }
                        return Expression.Call(left,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"名称为“Contains”的方法不存在"),
                            right);
                    }
                },
                {
                    FilterOperate.NotContains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“NotContains”比较方式只支持字符串类型的数据");
                        }
                        return Expression.Not(Expression.Call(left,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"名称为“Contains”的方法不存在"),
                            right));
                    }
                },
                {
                    FilterOperate.In, (left, right) =>
                    {
                        if (!right.Type.IsArray)
                        {
                            return null!;
                        }
                        var exp =  Expression.Call(typeof (Enumerable), "Contains", new[] {left.Type}, right, left);
                        return exp;
                    }
                }
        };

    private static Dictionary<Type, Func<JsonElement, object>> jsonElementConvertDic { get; set; } = new Dictionary<Type, Func<JsonElement, object>>()
    {
        {typeof(short),e=> e.GetInt16()},
        {typeof(short?),e=> e.GetInt16()},
        {typeof(int),e=> e.GetInt32()},
        {typeof(int?),e=> e.GetInt32()},
        {typeof(long),e=> e.GetInt64()},
        {typeof(long?),e=> e.GetInt64()},
        {typeof(decimal),e=> e.GetDecimal()},
        {typeof(decimal?),e=> e.GetDecimal()},
        {typeof(double?),e=> e.GetDouble()},
        {typeof(double),e=> e.GetDouble()},
        {typeof(string),e=> e.GetString()!},
        {typeof(DateTime),e=> e.GetDateTime()},
        {typeof(DateTimeOffset),e=> e.GetDateTimeOffset()},
        {typeof(bool),e=> e.GetBoolean()},
        {typeof(Guid),e=> e.GetGuid()}
    };

    #endregion

    /// <summary>
    /// 获取指定查询条件组的查询表达式
    /// </summary>
    /// <typeparam name="T">表达式实体类型</typeparam>
    /// <param name="rules">查询条件组，如果为null，则直接返回 true 表达式</param>
    /// <returns>查询表达式</returns>
    public static Expression<Func<T, bool>> GetExpression<T>(ICollection<FilterRule> rules, List<IQueryAction<T>> queryActions)
    {

        // 检查参数名
        var actionParams = queryActions?.Where(q => q.FilterExp != null).Select(q => q.FilterExp.Parameters.FirstOrDefault()).Where(p => p?.Name != null);
        if (actionParams != null && actionParams.GroupBy(p => p?.Name).Count() > 1)
            throw Oops.Oh(Excode.QUERY_ACTION_PARAM_ERROR);

        ParameterExpression param = actionParams?.FirstOrDefault() ?? Expression.Parameter(typeof(T), "m");
        var visitor = new ParameterReplacementVisitor(param);
        // 替换Param
        queryActions?.ForEach(d =>
        {
            if (d.FilterExp != null)
                d.FilterExp = (LambdaExpression)visitor.Visit(d.FilterExp);
        });

        Expression body = GetExpressionBody(param, rules, queryActions);
        Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(body, param);

        return expression;
    }

    #region 私有方法
    /// <summary>
    /// 根据 or 拆分
    /// </summary>
    /// <param name="rules"></param>
    /// <returns></returns>
    private static List<FilterGroup> Divide(List<FilterRule> rules)
    {
        if (rules == null || rules.Count == 0)
        {
            return new List<FilterGroup>();
        }
        List<FilterGroup> groups = new List<FilterGroup>();
        for (int i = 0; i < rules.Count; i++)
        {
            if (i == 0)
            {
                groups.Add(new FilterGroup().AddRule(rules[i]));
            }
            else
            {
                if (rules[i].Condition.Equals(FilterCondition.Or))
                {
                    groups.Add(new FilterGroup().AddRule(rules[i]));
                }
                else
                {
                    groups.Last().Rules.Add(rules[i]);
                }
            }

        }
        return groups;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    private static Expression GetExpressionBody<T>(ParameterExpression param, ICollection<FilterRule> rules, List<IQueryAction<T>> queryActions = null)
    {

        //如果无条件或条件为空，直接返回 true表达式
        if (rules == null || rules.Count == 0)
        {
            return Expression.Constant(true);
        }
        List<Expression> bodies = new List<Expression>();

        List<Expression> groupExps = new List<Expression>();
        List<FilterGroup> dGroups = Divide(rules.ToList());

        foreach (FilterGroup dgroup in dGroups)
        {
            var action = queryActions?.FirstOrDefault(q => q.FilterExp != null && dgroup.Rules.Any(r => r.Field.ToUpperFirstLetter() == q.Field.ToUpperFirstLetter()));

            if (dgroup.Rules.Count > 1)
            {
                var list = new List<Expression>();
                dgroup.Rules.ForEach(x =>
                {
                    var exp = GetExpressionBody(param, x, queryActions);
                    if (exp != null)
                        list.Add(exp);
                });
                if (!list.IsEmpty())
                    groupExps.Add(list.Aggregate(Expression.AndAlso));
            }
            else
            {
                var exp = GetExpressionBody(param, dgroup.Rules.First(), queryActions);
                if (exp != null)
                    groupExps.Add(exp);
            }
        }

        if (groupExps.IsEmpty())
            groupExps.Add(Expression.Constant(true));


        bodies.Add(groupExps.Aggregate(Expression.OrElse));

        return bodies.Aggregate(Expression.AndAlso);
    }

    private static bool IsValid(FilterRule rule)
    {
        if (rule == null || string.IsNullOrWhiteSpace(rule.Field) || string.IsNullOrWhiteSpace(rule.Value?.ToString()))
        {
            return false;
        }
        else
        {
            if (!Enum.IsDefined(rule.Operate))
                throw new Exception(@$"{rule.Field}下Operate不存在");
        }
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <param name="rule"></param>
    /// <returns></returns>
    public static Expression GetExpressionBody<T>(ParameterExpression param, FilterRule rule, List<IQueryAction<T>> queryActions = null)
    {
        if (!IsValid(rule)) return null;




        var action = queryActions?.FirstOrDefault(q => q.FilterExp != null && rule.Field.ToUpperFirstLetter() == q.Field.ToUpperFirstLetter());

        if (action != null)
        {
            Expression constant = ChangeTypeToExpression(rule, action.FilterExp.Body.Type);
            Expression body = action.FilterExp;
            var exp = ExpressionDict[rule.Operate](action.FilterExp.Body, constant);
            return exp;
        }
        else
        {
            return GetLambdaExpression(param, rule);
        }
    }


    class TempExpression
    {
        public TempExpression(Expression exp, ParameterExpression paramExp, Type linqType)
        {
            Exp = exp;
            ParamExp = paramExp;
            LinqType = linqType;
        }

        /// <summary>
        /// 表达式
        /// </summary>
        public Expression Exp { get; set; }
        /// <summary>
        /// 表达式参数
        /// </summary>
        public ParameterExpression ParamExp { get; set; }
        /// <summary>
        /// 表达式内参数类型
        /// </summary>
        public Type LinqType { get; set; }
    }

    private static Expression GetLambdaExpression(ParameterExpression param, FilterRule rule)
    {
        Expression exp = null;
        var fields = rule.Field.Split(".").ToList();

        var exps = new List<TempExpression>();

        var type = param.Type;

        Expression access = param;

        // 表达式树访问参数
        ParameterExpression collectionParamExp = null;
        for (int i = 0; i < fields.Count; i++)
        {

            var field = fields[i];
            PropertyInfo property = null;

            if (type.IsCollectionType())
            {
                // 存储上一个表达式
                exps.Add(new TempExpression(access, collectionParamExp, type.GetGenericFirstType()));

                // 集合内部的泛型类型
                var collectionGenType = type.GetGenericFirstType();

                // 当前字段PropertyInfo
                property = collectionGenType.GetProperty(field, true);

                // 生成参数名，示例：A
                string paramStr = Encoding.ASCII.GetString(new byte[] { (byte)(64 + i) });
                collectionParamExp = Expression.Parameter(collectionGenType, paramStr);

                // 访问参数 示例：A.Id
                access = Expression.MakeMemberAccess(i == 0 ? param : collectionParamExp, property);
            }
            else
            {
                property = type.GetProperty(field, true);
                if (property == null)
                    throw Oops.Oh(Excode.FIELD_IN_TYPE_NOT_FOUND, @$"{rule.Field}中的{field}", type.FullName);

                access = Expression.MakeMemberAccess(access, property);
            }
            type = property.PropertyType;

        }


        // 遍历完后，access值一定是最后的表达式
        var lambdaExp = Expression.Lambda(access, collectionParamExp ?? param);

        Expression constant = ChangeTypeToExpression(rule, lambdaExp.Body.Type);
        exp = ExpressionDict[rule.Operate](lambdaExp.Body, constant);

        if (!exps.IsEmpty())
        {
            for (int i = exps.Count - 1; i >= 0; i--)
            {
                var tExp = exps[i].Exp;

                // 生成lamda
                var lamd = Expression.Lambda(exp, i == exps.Count - 1 ? collectionParamExp : exps[i + 1].ParamExp ?? param);

                exp = Expression.Call(typeof(Enumerable), "Any", new[] { exps[i].LinqType }, tExp, lamd);
            }
        }

        return exp;

    }


    /// <summary>
    /// 验证属性与属性值是否匹配 
    /// </summary>
    /// <param name="type">最后一个属性</param>
    /// <param name="rule">条件信息</param>
    /// <returns></returns>
    //private static bool CheckFilterRule(Type type, FilterRule rule)
    //{
    //    if (rule.Value == null || rule.Value.ToString() == string.Empty)
    //    {
    //        rule.Value = null;
    //    }

    //    if (rule.Value == null && (type == typeof(string) || ObjectHelper.IsNullableType(type)))
    //    {
    //        return rule.Operate == FilterOperate.Equal || rule.Operate == FilterOperate.NotEqual;
    //    }

    //    if (rule.Value == null)
    //    {
    //        return !type.IsValueType;
    //    }
    //    return true;
    //}

    private static Expression ChangeTypeToExpression(FilterRule rule, Type conversionType)
    {
        if (rule.Operate.Equals(FilterOperate.In))
        {
            List<Expression> expressionList = new List<Expression>();
            if (rule.Value is JsonElement)
            {
                JsonElement values = (JsonElement)rule.Value;
                if (values.ValueKind.Equals(JsonValueKind.Array))
                {
                    if (jsonElementConvertDic.ContainsKey(conversionType))
                    {
                        foreach (JsonElement e in values.EnumerateArray())
                        {
                            expressionList.Add(Expression.Constant(jsonElementConvertDic[conversionType].Invoke(e), conversionType));
                        }
                    }
                    else
                    {
                        throw Oops.Oh(Excode.QUERY_VALUE_TYPE_NO_FIND_CONVERTER, conversionType.Name);
                    }

                }
            }
            else if (rule.Value is JArray)
            {
                foreach (var item in (JArray)rule.Value)
                {
                    var dt = JsonConvert.DeserializeObject(item.ToString(), conversionType);
                    expressionList.Add(Expression.Constant(dt, conversionType));
                }
            }
            else if (rule.Value is Array)
            {
                foreach (var item in (Array)rule.Value)
                {
                    expressionList.Add(Expression.Constant(item, conversionType));
                }
            }
            // 字符串序列化，该方式不需要，前端传值会自动序列化
            //else if (rule.Value is string)
            //{
            //    var list = JsonConvert.DeserializeObject(rule.Value.ToString());
            //    foreach (var item in (JArray)list)
            //    {
            //        var dt = JsonConvert.DeserializeObject(item.ToString(), conversionType);
            //        expressionList.Add(Expression.Constant(dt, conversionType));
            //    }
            //}
            return Expression.NewArrayInit(conversionType, expressionList);
        }
        else if (rule.Value is JsonElement)
        {
            JsonElement json = (JsonElement)rule.Value;
            object value = null;
            //枚举
            if (conversionType.IsEnum)
            {
                value = Enum.ToObject(conversionType, json.GetInt64());
            }
            else
            {
                value = jsonElementConvertDic[conversionType].Invoke(json);
            }
            return Expression.Constant(value, conversionType);
        }
        else
        {
            object value = ObjectHelper.CastTo(rule.Value!, conversionType);
            return Expression.Constant(value, conversionType);
        }
    }
    #endregion
}
