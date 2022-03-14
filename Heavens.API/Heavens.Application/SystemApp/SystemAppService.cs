using Heavens.Core.Extension.SearchEngine;
using Meilisearch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Application.SystemApp;
[AllowAnonymous]
public class SystemAppService : IDynamicApiController
{
    public SystemAppService(ISearchEngine searchEngine)
    {
        _searchEngine = searchEngine;
    }

    public string version { get; set; }
    public ISearchEngine _searchEngine { get; }

    /// <summary>
    /// 获取后台版本号
    /// </summary>
    /// <returns></returns>
    public string GetVersion()
    {
        if (version.IsEmpty())
        {
            System.Version v = Assembly.GetEntryAssembly().GetFileVersion();
            version = v.ToString();
        }
        return version;
    }

    /// <summary>
    /// 获取搜索引擎 Search key
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetSearchEngineKey()
    {
        return (await _searchEngine.GetSearchKey())?.KeyUid;
    }

}
