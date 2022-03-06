using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.Extensions;

public static partial class ExpressionExtension
{
    /// <summary>
    /// 返回被转换后的Lambda表达式字符串
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    public static string? ToLambdaString(this Expression exp)
    {
        return exp?.ToString()?.Replace("OrElse", "||").Replace("AndAlso", "&&").Replace("Not","!");
    }
}
