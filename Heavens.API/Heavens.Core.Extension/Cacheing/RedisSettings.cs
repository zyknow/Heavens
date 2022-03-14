namespace Heavens.Core.Extension.Cacheing;

public class RedisSettings
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    public List<string> ConnectionStrings { get; set; }
    /// <summary>
    /// 哨兵连接字符串
    /// </summary>
    public List<string> Sentinels { get; set; }
    /// <summary>
    /// 哨兵只读
    /// </summary>
    public bool SentinelReadOnly { get; set; }
}
