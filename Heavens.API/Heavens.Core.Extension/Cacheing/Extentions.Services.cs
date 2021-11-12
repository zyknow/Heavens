using Bing.EasyCaching;
using EasyCaching.CSRedis;
using Furion;
using Microsoft.Extensions.DependencyInjection;

namespace Heavens.Core.Extension.Cacheing
{
    public static class OptionsServicesExtentions
    {
        /// <summary>
        /// Options扩展
        /// </summary>
        /// <param name="services"></param>
        public static void AddOptionServices(this IServiceCollection services)
        {
            services.AddConfigurableOptions<CacheingOptions>();
        }

        /// <summary>
        /// 添加缓存Services
        /// </summary>
        /// <param name="services"></param>
        public static void AddCache(this IServiceCollection services)

        {
            CacheingOptions cacheOpt = App.GetConfig<CacheingOptions>("Cacheing");

            switch (cacheOpt.CacheMode)
            {
                case CacheModeEnum.InMemory: // 内存缓存，默认
                default:
                    services.AddCaching(options => options.UseInMemory());
                    break;
                case CacheModeEnum.Redis:
                    services.AddCaching(options =>
                        options.UseCSRedis(conf =>
                            conf.DBConfig = new CSRedisDBOptions
                            {
                                ConnectionStrings = cacheOpt.RedisSettings.ConnectionStrings,
                                Sentinels = cacheOpt.RedisSettings.Sentinels,
                                ReadOnly = cacheOpt.RedisSettings.SentinelReadOnly
                            }
                        )
                    );
                    break;
                case CacheModeEnum.SQLite:
                    break;
                case CacheModeEnum.Memcached:
                    break;
            }


        }
    }
}