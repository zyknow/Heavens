using Furion.FriendlyException;
using Heavens.Core.Extension.PageQueayFilter.common;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Heavens.Core.Extension.PageQueayFilter.helper;

/// <summary>
/// 查询表达式辅助操作类
/// </summary>
public static class FilterHelper
{
    #region 字段

    private static readonly Dictionary<FilterOperate, Func<Expression, Expression, Expression>> ExpressionDict =
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
                            return null;
                        }
                        return Expression.Call(typeof (Enumerable), "Contains", new[] {left.Type}, right, left);
                    }
                }
        };

    private static readonly Dictionary<Type, Func<JsonElement, object>> jsonElementConvertDic = new Dictionary<Type, Func<JsonElement, object>>()
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
            {typeof(string),e=> e.GetString()},
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
    public static Expression<Func<T, bool>> GetExpression<T>(ICollection<FilterRule> rules)
    {
        ParameterExpression param = Expression.Parameter(typeof(T), "m");
        Expression body = GetExpressionBody(param, rules);
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
            return null;
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
    private static Expression GetExpressionBody(ParameterExpression param, ICollection<FilterRule> rules)
    {

        //如果无条件或条件为空，直接返回 true表达式
        if (rules == null || rules.Count == 0)
        {
            return Expression.Constant(true);
        }
        List<Expression> bodies = new List<Expression>();

        List<Expression> groupExs = new List<Expression>();
        List<FilterGroup> dGroups = Divide(rules.ToList());

        foreach (FilterGroup dgroup in dGroups)
        {

            if (dgroup.Rules.Count > 1)
            {
                groupExs.Add(dgroup.Rules.Select(x => GetExpressionBody(param, x)).Aggregate(Expression.AndAlso));
            }
            else
            {
                groupExs.Add(GetExpressionBody(param, dgroup.Rules.First()));
            }

        }
        bodies.Add(groupExs.Aggregate(Expression.OrElse));

        return bodies.Aggregate(Expression.AndAlso);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <param name="rule"></param>
    /// <returns></returns>
    private static Expression GetExpressionBody(ParameterExpression param, FilterRule rule)
    {
        // if (rule == null || rule.Value == null || string.IsNullOrEmpty(rule.Value.ToString()))
        if (rule == null)
        {
            return Expression.Constant(true);
        }
        LambdaExpression expression = GetPropertyLambdaExpression(param, rule);
        if (expression == null)
        {
            return Expression.Constant(true);
        }
        Expression constant = ChangeTypeToExpression(rule, expression.Body.Type);
        return ExpressionDict[rule.Operate](expression.Body, constant);
    }

    private static LambdaExpression GetPropertyLambdaExpression(ParameterExpression param, FilterRule rule)
    {
        Expression propertyAccess = param;
        Type type = param.Type;
        string propertyName = rule.Field;
        PropertyInfo property = type.GetProperty(propertyName) ?? type.GetProperty(propertyName.ToUpperFirstLetter());
        if (property == null)
        {
            throw Oops.Oh(Excode.FIELD_IN_TYPE_NOT_FOUND, propertyName, type.FullName);
        }
        //验证属性与属性值是否匹配
        bool flag = CheckFilterRule(property.PropertyType, rule);
        if (!flag)
        {
            return null;
        }
        propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
        return Expression.Lambda(propertyAccess, param);
    }

    /// <summary>
    /// 验证属性与属性值是否匹配 
    /// </summary>
    /// <param name="type">最后一个属性</param>
    /// <param name="rule">条件信息</param>
    /// <returns></returns>
    private static bool CheckFilterRule(Type type, FilterRule rule)
    {
        if (rule.Value == null || rule.Value.ToString() == string.Empty)
        {
            rule.Value = null;
        }

        if (rule.Value == null && (type == typeof(string) || ObjectHelper.IsNullableType(type)))
        {
            return rule.Operate == FilterOperate.Equal || rule.Operate == FilterOperate.NotEqual;
        }

        if (rule.Value == null)
        {
            return !type.IsValueType;
        }
        return true;
    }

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
                    //if (conversionType.IsNullableType()) 
                    //{
                    //    conversionType = conversionType.GetUnNullableType();
                    //}
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
            object value = ObjectHelper.CastTo(rule.Value, conversionType);
            return Expression.Constant(value, conversionType);
        }
    }
    #endregion
}
