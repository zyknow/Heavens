using Heavens.Core.Extension.QueayFilter.attributes;

namespace Heavens.Core.Extension.QueayFilter.helper;

/// <summary>
/// 枚举操作类
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// 获取枚举描述
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string GetEnumCode(Enum item)
    {
        object[] attrs = item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(CodeAttribute), true);
        if (attrs != null && attrs.Length > 0)
        {
            CodeAttribute descAttr = attrs[0] as CodeAttribute;
            return descAttr.Code;
        }
        return null;
    }
}
