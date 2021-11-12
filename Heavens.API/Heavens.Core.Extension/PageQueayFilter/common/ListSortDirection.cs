namespace Heavens.Core.Extension.PageQueayFilter.common;

/// <summary>
/// 搜索排序
/// </summary>
public class ListSortDirection
{
    /// <summary>
    /// 字段名
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// 0 asc 
    /// 1 desc
    /// </summary>
    public ListSortType SortType { get; set; } = ListSortType.Desc;
}

/// <summary>
/// 排序类型
/// </summary>
public enum ListSortType
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
