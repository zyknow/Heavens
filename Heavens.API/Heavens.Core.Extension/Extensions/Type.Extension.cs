using System.Reflection;
namespace Heavens.Core.Extension.Extensions;

public static partial class TypeExtension
{
    /// <summary>
    /// 获取PropertyInfo
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ignoreCase">是否忽略大小写</param>
    /// <returns></returns>
    public static PropertyInfo GetProperty(this Type type, string name, bool ignoreCase)
    {

        ArgumentNullException.ThrowIfNull(type);

        if (!ignoreCase)
            return type.GetProperty(name);

        var info = type.GetProperty(name) ?? type.GetProperty(name.ToUpperFirstLetter());

        ArgumentNullException.ThrowIfNull(info);

        return info;
    }

    /// <summary>
    /// 获取集合类型中的泛型类型
    /// 如果不是集合类型，则直接返回type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetGenericFirstType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsCollectionType() ? type.GetGenericArguments().First() : type;

    }
}
