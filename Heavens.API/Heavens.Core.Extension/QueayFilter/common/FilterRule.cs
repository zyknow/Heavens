using System.ComponentModel;

namespace Heavens.Core.Extension.QueayFilter.common;

/// <summary>
/// 筛选规则
/// </summary>
public class FilterRule
{
    public FilterRule()
    {
    }

    /// <summary>
    /// 使用指定数据名称，数据值及操作方式初始化一个<see cref="FilterRule"/>的新实例
    /// </summary>
    /// <param name="field">数据名称</param>
    /// <param name="value">数据值</param>
    /// <param name="operate">操作方式</param>
    public FilterRule(string field, object value, FilterOperate operate)
    {
        Field = field;
        Value = value;
        Operate = operate;
    }

    public FilterRule(string field, object value, FilterOperate operate, FilterCondition condition) : this(field, value, operate)
    {
        Condition = condition;
    }

    /// <summary>
    /// 获取或设置 属性名称
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 属性值
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// 获取或设置 操作类型
    /// </summary>
    public FilterOperate Operate { get; set; }

    /// <summary>
    /// 获取或设置 条件间操作方式，仅限And, Or
    /// </summary>
    public FilterCondition Condition { get; set; } = FilterCondition.Or;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Equals(FilterRule obj)
    {
        return obj.Field == Field && obj.Value == Value && obj.Operate == Operate;
    }
}

public enum FilterCondition
{
    /// <summary>
    /// 或者
    /// </summary>
    [Description("或者")]
    Or = 1,
    /// <summary>
    /// 并且
    /// </summary>
    [Description("并且")]
    And = 2


}

/// <summary>
/// 筛选操作方式
/// </summary>
public enum FilterOperate
{
    /// <summary>
    /// 等于
    /// </summary>
    [Description("等于")]
    Equal = 3,

    /// <summary>
    /// 不等于
    /// </summary>
    [Description("不等于")]
    NotEqual = 4,

    /// <summary>
    /// 小于
    /// </summary>
    [Description("小于")]
    Less = 5,

    /// <summary>
    /// 小于或等于
    /// </summary>
    [Description("小于等于")]
    LessOrEqual = 6,

    /// <summary>
    /// 大于
    /// </summary>
    [Description("大于")]
    Greater = 7,

    /// <summary>
    /// 大于或等于
    /// </summary>
    [Description("大于等于")]
    GreaterOrEqual = 8,

    /// <summary>
    /// 以……开始
    /// </summary>
    [Description("开始于")]
    StartsWith = 9,

    /// <summary>
    /// 以……结束
    /// </summary>
    [Description("结束于")]
    EndsWith = 10,

    /// <summary>
    /// 字符串的包含（相似）
    /// </summary>
    [Description("包含")]
    Contains = 11,

    /// <summary>
    /// 字符串的不包含
    /// </summary>
    [Description("不包含")]
    NotContains = 12,
    /// <summary>
    /// 包括在
    /// </summary>
    [Description("包括在")]
    In = 13,

    ///// <summary>
    ///// 不包括在
    ///// </summary>
    //[Code("notin")]
    //[Description("不包括在")]
    //NotIn = 14
}
