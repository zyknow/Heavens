using Furion.Extensions;
using Furion.FriendlyException;
using System.ComponentModel;

namespace Heavens.Core.Extension.QueayFilter;

/// <summary>
/// Object拓展方法，.Net4.0以上
/// </summary>
internal class ObjectHelper
{
    /// <summary>
    /// 把对象类型转换为指定类型
    /// </summary>
    /// <param name="value"></param>
    /// <param name="conversionType"></param>
    /// <returns></returns>
    public static object? CastTo(object value, Type conversionType)
    {
        if (value == null)
        {
            return null;
        }
        if (conversionType.IsNullableType())
        {
            conversionType = GetUnNullableType(conversionType);
        }
        if (conversionType.IsEnum)
        {
            return Enum.Parse(conversionType, value.ToString()!);
        }
        if (conversionType == typeof(Guid))
        {
            return Guid.Parse(value.ToString()!);
        }

        if(conversionType == typeof(DateTimeOffset) && value.GetType() == typeof(DateTime))
            return ((DateTime)value).ConvertToDateTimeOffset();

        try
        {
            return Convert.ChangeType(value, conversionType);
        }
        catch (Exception)
        {
            throw Oops.Oh(@$"类型转换失败，无法将[{value}]转换为{conversionType.Name}");
        }
        
    }

    /// <summary>
    /// 通过类型转换器获取Nullable类型的基础类型
    /// </summary>
    /// <param name="type"> 要处理的类型对象 </param>
    /// <returns> </returns>
    public static Type GetUnNullableType(Type type)
    {
        if (IsNullableType(type))
        {
            NullableConverter nullableConverter = new NullableConverter(type);
            return nullableConverter.UnderlyingType;
        }
        return type;
    }

    /// <summary>
    /// 判断类型是否为Nullable类型
    /// </summary>
    /// <param name="type"> 要处理的类型 </param>
    /// <returns> 是返回True，不是返回False </returns>
    public static bool IsNullableType(Type type)
    {
        return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// 把对象类型转化为指定类型
    /// </summary>
    /// <typeparam name="T"> 动态类型 </typeparam>
    /// <param name="value"> 要转化的源对象 </param>
    /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
    public static T? CastTo<T>(object value)
    {
        if (value == null && default(T) == null)
        {
            return default;
        }
        if (value!.GetType() == typeof(T))
        {
            return (T)value;
        }
        object? result = CastTo(value, typeof(T));


        return (T)result!;
    }

}
