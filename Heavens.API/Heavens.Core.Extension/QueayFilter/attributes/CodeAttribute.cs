namespace Heavens.Core.Extension.QueayFilter.attributes;

/// <summary>
/// 操作码
/// </summary>
public class CodeAttribute : Attribute
{
    /// <summary>
    /// 初始化一个<see cref="CodeAttribute"/>类型的新实例
    /// </summary>
    public CodeAttribute(string code)
    {
        Code = code;
    }

    /// <summary>
    /// 获取 属性名称
    /// </summary>
    public string Code { get; private set; }
}
