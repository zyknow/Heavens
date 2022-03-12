using Furion.ConfigurableOptions;

namespace Heavens.Core.Extension.Cacheing;

/// <summary>
/// 缓存设置选项
/// </summary>
public class CacheingOptions : IConfigurableOptions
{
    /// <summary>
    /// 缓存模式
    /// </summary>
    public CacheModeEnum CacheMode { get; set; }
    /// <summary>
    /// redis设置(如果缓存模式选的redis，该配置必填)
    /// </summary>
    public RedisSettings? RedisSettings { get; set; }

}

public enum CacheModeEnum
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    InMemory,
    /// <summary>
    /// redis缓存
    /// </summary>
    Redis,
    /// <summary>
    /// SQLite缓存
    /// </summary>
    SQLite,
    /// <summary>
    /// 分布式缓存
    /// </summary>
    Memcached
}
