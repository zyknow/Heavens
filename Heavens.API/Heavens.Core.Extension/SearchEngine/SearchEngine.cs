using Furion;
using Meilisearch;
using Microsoft.Extensions.Logging;

namespace Heavens.Core.Extension.SearchEngine;

public class SearchEngine : ISearchEngine
{
    MeilisearchClient? client;

    public SearchEngineOptions option { get; set; }

    public SearchEngine(ILogger<SearchEngine> logger)
    {
        _logger = logger;
        InitAsync();
        option = App.GetConfig<SearchEngineOptions>("SearchEngineSettings");
    }

    public ILogger<SearchEngine> _logger { get; }

    private async void InitAsync()
    {
        await Connect();
    }

    /// <summary>
    /// 连接搜索引擎
    /// </summary>
    /// <param name="connectStr"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> Connect()
    {
        if (option?.Enabled != true)
            return false;

        client = new MeilisearchClient(option.ConnectStr, option.MasterKey);
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
        if (client == null)
        {
            return Task.FromResult(false);
        }

        return client.IsHealthyAsync();
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
        //if (option?.Enabled != true)
        //    return false;

        Meilisearch.Index? index = await client?.GetIndexAsync(indexStr)!;
        if (index == null)
            await client.CreateIndexAsync(indexStr);
        index = await client.GetIndexAsync(indexStr);

        return await index.SearchAsync<T>(search);
    }

    /// <summary>
    /// 添加/更新
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexStr"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public async Task AddOrUpdates<T>(string indexStr, List<T>? list = null) where T : class, new()
    {
        //if (option?.Enabled != true)
        //    return false;
        //BeforeCheck();
        Meilisearch.Index index = await client?.GetIndexAsync(indexStr)!;
        if (index == null)
            await client.CreateIndexAsync(indexStr);
        index = await client.GetIndexAsync(indexStr);

        if (!list.IsNullOrEmpty())
        {
            await index.AddDocumentsAsync(list);
        }
    }

    /// <summary>
    /// 添加/更新
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexStr"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task AddOrUpdate<T>(string indexStr, T? entity = null) where T : class, new()
    {
        //if (option?.Enabled != true)
        //    return false;
        //BeforeCheck();
        Meilisearch.Index index = await client?.GetIndexAsync(indexStr)!;
        if (index == null)
            await client.CreateIndexAsync(indexStr);
        index = await client.GetIndexAsync(indexStr);

        if (entity != null)
        {
            await index.AddDocumentsAsync(new List<T>() { entity });
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
        //if (option?.Enabled != true)
        //    return false;
        //BeforeCheck();
        Meilisearch.Index index = await client?.GetIndexAsync(indexStr)!;
        if (index == null)
            await client.CreateIndexAsync(indexStr);
        index = await client.GetIndexAsync(indexStr);

        await index.DeleteDocumentsAsync(ids);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="indexStr"></param>
    /// <returns></returns>
    public async Task DeleteIndex(string indexStr)
    {
        //if (option?.Enabled != true)
        //    return false;
        //BeforeCheck();
        await client!.DeleteIndexAsync(indexStr);
    }

    /// <summary>
    /// 删除所有Index
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAllIndex()
    {

        //if (option?.Enabled != true)
        //    return false;
        //BeforeCheck();
        IEnumerable<Meilisearch.Index> indexs = await client?.GetAllIndexesAsync()!;
        foreach (Meilisearch.Index index in indexs)
        {
            await index.DeleteAsync();
        }
    }
}
