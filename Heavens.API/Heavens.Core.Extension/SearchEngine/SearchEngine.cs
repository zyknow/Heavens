using Furion;
using Meilisearch;
using Microsoft.Extensions.Logging;

namespace Heavens.Core.Extension.SearchEngine;

public class SearchEngine : ISearchEngine
{
    MeilisearchClient client;

    public SearchEngine(ILogger<SearchEngine> logger)
    {
        _logger = logger;
        Connect();
    }

    public ILogger<SearchEngine> _logger { get; }

    /// <summary>
    /// 配置文件是否开启搜索引擎
    /// </summary>
    private bool _enabled => App.Configuration["SearchEngineSettings:Enabled"].ToBool();

    /// <summary>
    /// 连接搜索引擎
    /// </summary>
    /// <param name="connectStr"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> Connect(string connectStr = null, string key = null)
    {
        if (!_enabled)
        {
            return false;
        }

        connectStr = connectStr ?? App.Configuration["SearchEngineSettings:ConnectStr"];
        key = key ?? App.Configuration["SearchEngineSettings:MasterKey"];

        client = new MeilisearchClient(connectStr, key);
        bool healthy = await IsHealthy();
        _logger.LogInformation(@$"搜索引擎连接：{(healthy ? "成功" : "失败")}");
        return healthy;
        //try
        //{
        //    client = new MeilisearchClient(connectStr, key);
        //    var healthy =  await client.IsHealthy();
        //    return (true, null);
        //}
        //catch (Exception e)
        //{
        //    _logger.LogError("连接搜索引擎失败", e);
        //    return (false, e.ToString());
        //}
    }

    /// <summary>
    /// 获取搜索引擎状态
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsHealthy()
    {
        if (client.IsNull())
        {
            return Task.FromResult(false);
        }

        return client?.IsHealthy();
    }

    /// <summary>
    /// 搜索
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexStr"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<SearchResult<T>> Search<T>(string indexStr, string search)
    {
        if (!_enabled)
        {
            return default;
        }

        Meilisearch.Index index = await client.GetOrCreateIndex(indexStr);
        return await index.Search<T>(search);
    }

    /// <summary>
    /// 添加/更新
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexStr"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public async Task AddOrUpdates<T>(string indexStr, List<T> list = null) where T : class, new()
    {
        if (!_enabled)
        {
            return;
        }
        //BeforeCheck();
        Meilisearch.Index index = await client.GetOrCreateIndex(indexStr);
        if (!list.IsNullOrEmpty())
        {
            await index.AddDocuments(list);
        }
    }

    /// <summary>
    /// 添加/更新
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexStr"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task AddOrUpdate<T>(string indexStr, T entity = null) where T : class, new()
    {
        if (!_enabled)
        {
            return;
        }
        //BeforeCheck();
        Meilisearch.Index index = await client.GetOrCreateIndex(indexStr);
        if (!entity.IsNull())
        {
            await index.AddDocuments(new List<T>() { entity });
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="indexStr"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task DeleteDocuments(string indexStr, string[] ids)
    {
        if (!_enabled)
        {
            return;
        }
        //BeforeCheck();
        Meilisearch.Index index = await client.GetOrCreateIndex(indexStr);
        await index.DeleteDocuments(ids);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="indexStr"></param>
    /// <returns></returns>
    public async Task DeleteIndex(string indexStr)
    {
        if (!_enabled)
        {
            return;
        }
        //BeforeCheck();
        await client.DeleteIndex(indexStr);
    }

    /// <summary>
    /// 删除所有Index
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAllIndex()
    {

        if (!_enabled)
        {
            return;
        }
        //BeforeCheck();
        IEnumerable<Meilisearch.Index> indexs = await client.GetAllIndexes();
        foreach (Meilisearch.Index index in indexs)
        {
            await index.Delete();
        }
    }

    private void BeforeCheck()
    {
        if (client == null)
        {
            Connect();
        }
    }
}
