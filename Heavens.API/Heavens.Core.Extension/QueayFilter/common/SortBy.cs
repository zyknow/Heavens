namespace Heavens.Core.Extension.QueayFilter.common;

/// <summary>
/// 搜索排序
/// </summary>
public class SortBy
{
    /// <summary>
    /// 字段名
    /// </summary>
    public string Field { get; set; }
    /// <summary>
    /// 0 asc 
    /// 1 desc
    /// </summary>
    public SortType SortType { get; set; } = SortType.Desc;
}

/// <summary>
/// 排序类型
/// </summary>
public enum SortType
{
    /// <summary>
    /// 倒序
    /// </summary>
    Desc,
    /// <summary>
    /// 正序
    /// </summary>
    Asc
}
