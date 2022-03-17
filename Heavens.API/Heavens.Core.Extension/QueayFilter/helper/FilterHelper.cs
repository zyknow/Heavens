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
            var fields = rule.Field.Split(".");
            LambdaExpression expression;
            if (fields.Length == 1)
            {
                expression = GetPropertyLambdaExpression(param, rule);

                Expression constant = ChangeTypeToExpression(rule, expression.Body.Type);
                var exp = ExpressionDict[rule.Operate](expression.Body, constant);
                return exp;
            }
            else
                return GetMultiPropertyLambdaExpression(param, rule, fields.ToList())?.Body;
        }
    }

    private static LambdaExpression GetMultiPropertyLambdaExpression(ParameterExpression param, FilterRule rule, List<string> fields)
    {
        Expression exp = null;

        List<Type> accessTypes = new List<Type>();

        List<Type> originTyps = new List<Type>();

        Expression lastFieldExp = null;

        #region 获取所有字段类型Type


        // 上一个访问字段类型
        var prevAccessType = param.Type;
        foreach (var field in fields)
        {
            var type = prevAccessType.GetProperty(field, true).PropertyType;

            prevAccessType = type.GetGenericFirstType();

            accessTypes.Add(prevAccessType);
            originTyps.Add(type);
        }
        #endregion

        // 生成最后面的表达式
        if(originTyps[^2].IsCollectionType())
        {
            int i = fields.Count - 1;
            // 字段名
            var field = fields[i];

            // 字段类型
            var type = accessTypes[i];

            // 检测第一个字段类型
            bool flag = CheckFilterRule(type, rule);
            if (!flag)
                return null;

            // 获取上级类型
            var parentType = i == 0 ? param.Type : accessTypes[i - 1];
            var parentOriginType = i == 0 ? param.Type : originTyps[i - 1];



            // 当前字段PropertyInfo
            PropertyInfo property = parentType.GetProperty(field, true);

            // 生成参数名，示例：A
            string paramStr = Encoding.ASCII.GetString(new byte[] { (byte)(64 + i) });
            ParameterExpression parameterExpression = Expression.Parameter(parentType, paramStr);

            // 访问参数 示例：A.Id
            var access = Expression.MakeMemberAccess( parameterExpression, property);

            Expression expression = null;
    
                // 获取操作数据
                var value = ChangeTypeToExpression(rule, property.PropertyType);
                expression = ExpressionDict[rule.Operate](access, value);
                // 生成expBody lamda 示例：A=> A.Id == 1
                //exp = Expression.Lambda(expression, parameterExpression);
 
            if (parentOriginType.IsCollectionType())
            {
                lastFieldExp = Expression.Lambda(expression, parameterExpression);
            }
            else
            {
                lastFieldExp = Expression.MakeMemberAccess(expression, property);
            }
        }

        // 倒叙遍历
        for (int i = 0; i < fields.Count; i++)
        {
            // 字段名
            var field = fields[i];

            // 字段类型
            var type = accessTypes[i];

            // 检测第一个字段类型
            if (i == fields.Count - 1)
            {
                bool flag = CheckFilterRule(type, rule);
                if (!flag)
                    return null;
            }

            // 获取上级类型
            var parentType = i == 0 ? param.Type : accessTypes[i - 1];
            var parentOriginType = i == 0 ? param.Type : originTyps[i - 1];



            // 当前字段PropertyInfo
            PropertyInfo property = parentType.GetProperty(field, true);

            // 生成参数名，示例：A
            string paramStr = Encoding.ASCII.GetString(new byte[] { (byte)(64 + i) });
            ParameterExpression parameterExpression = Expression.Parameter(parentType, paramStr);

            // 访问参数 示例：A.Id
            var access = Expression.MakeMemberAccess(i == 0 ? param : parameterExpression, property);

            Expression expression = null;
            if (i == fields.Count - 1)
            {
                // 获取操作数据
                var value = ChangeTypeToExpression(rule, property.PropertyType);
                expression = ExpressionDict[rule.Operate](access, value);
                // 生成expBody lamda 示例：A=> A.Id == 1
                //exp = Expression.Lambda(expression, parameterExpression);
            }
            else
            {
                if (property.PropertyType.IsCollectionType())
                {
                    expression = Expression.Call(typeof(Enumerable), "Any", new[] { type }, access, exp);
                    exp = Expression.Lambda(expression, i == 0 ? param : parameterExpression);
                }
                //else
                //{
                //    if (i == 0)
                //        exp = Expression.Lambda(exp, param);
                //    else
                //    {
                //        exp = Expression.Lambda(exp, parameterExpression);
                //    }
                //}
            }
            if (parentOriginType.IsCollectionType())
            {
                exp = Expression.Lambda(expression, parameterExpression);
            }
            else
            {
                exp = Expression.MakeMemberAccess(exp?? expression, property);
            }

        }
        return (LambdaExpression)exp;
    }


    private static LambdaExpression GetPropertyLambdaExpression(ParameterExpression param, FilterRule rule)
    {
        LambdaExpression exp = null;
        Expression propertyAccess = param;
        Type type = param.Type;

        var fields = rule.Field.Split(".");

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];


            var pTypeIsCollectionType = type.IsCollectionType();
            // 获取集合类型中的类型
            type = pTypeIsCollectionType ? type.GetGenericArguments().First() : type;

            PropertyInfo property = type.GetProperty(field) ?? type.GetProperty(field.ToUpperFirstLetter())!;
            if (property == null)
                throw Oops.Oh(Excode.FIELD_IN_TYPE_NOT_FOUND, @$"{rule.Field}中的{field}", type.FullName);

            // 最后一个属性验证属性与属性值是否匹配
            if (i == fields.Length - 1)
            {
                bool flag = CheckFilterRule(property.PropertyType, rule);
                if (!flag)
                    return null;
            }

            // 父类型为集合类型
            if (pTypeIsCollectionType)
            {
                // 生成参数
                ParameterExpression parameterExpression = Expression.Parameter(type, "v");

                // 生成访问参数
                var access = Expression.MakeMemberAccess(parameterExpression, property);

                // 获取操作数据
                var value = ChangeTypeToExpression(rule, property.PropertyType);

                // 
                var expr = ExpressionDict[rule.Operate](access, value);

                // 生成lamda
                var lamd = Expression.Lambda(expr, parameterExpression);

                // 拼接到Any里
                propertyAccess = Expression.Call(typeof(Enumerable), "Any", new[] { type }, propertyAccess, lamd);

            }
            else
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);

            type = property.PropertyType;
        }

        exp = Expression.Lambda(propertyAccess, param);
        return exp;
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
