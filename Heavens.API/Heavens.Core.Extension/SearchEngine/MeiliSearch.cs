using Furion;
using Furion.DependencyInjection;
using Meilisearch;
using Microsoft.Extensions.Logging;

namespace Heavens.Core.Extension.SearchEngine;

public class MeiliSearch : ISearchEngine,ISingleton
{
    public MeilisearchClient Client { get; set; }

    public SearchEngineOptions option { get; set; }

    public MeiliSearch(ILogger<MeiliSearch> logger)
    {
        _logger = logger;
        option = App.GetConfig<SearchEngineOptions>("SearchEngineSettings");
        Init();

    }

    public ILogger<MeiliSearch> _logger { get; }

    private async void Init()
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
        {
            Console.WriteLine("未开启搜索引擎");
            return false;
        }

        Client = new MeilisearchClient(option.ConnectStr, option.MasterKey);
        bool healthy = await IsHealthy();
        _logger.LogInformation(@$"搜索引擎连接：{(healthy ? "成功" : "失败")}");
        return healthy;
    }

    /// <summary>
    /// 搜索引擎状态
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsHealthy()
    {
        ArgumentNullException.ThrowIfNull(Client);
        return await Client.IsHealthyAsync()!;


    }

    /// <summary>
    /// 搜索
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexStr"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<SearchResult<T>> Search<T>(string indexStr, string search, SearchQuery searchQuery = null)
    {
        Meilisearch.Index index = await Client?.GetIndexAsync(indexStr)!;
        ArgumentNullException.ThrowIfNull(index);

        return await index.SearchAsync<T>(search, searchQuery);
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

        Meilisearch.Index index = Client?.Index(indexStr);
        ArgumentNullException.ThrowIfNull(index);

        if (!list.IsEmpty())
            await index.AddDocumentsAsync(list);
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

        Meilisearch.Index index = Client?.Index(indexStr)!;
        ArgumentNullException.ThrowIfNull(index);

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
        Meilisearch.Index index = await Client?.GetIndexAsync(indexStr)!;
        ArgumentNullException.ThrowIfNull(index);

        await index.DeleteDocumentsAsync(ids);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="indexStr"></param>
    /// <returns></returns>
    public async Task DeleteIndex(string indexStr)
    {
        ArgumentNullException.ThrowIfNull(Client);
        await Client.DeleteIndexAsync(indexStr);
    }

    /// <summary>
    /// 删除所有Index
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAllIndex()
    {
        IEnumerable<Meilisearch.Index> indexs = await Client?.GetAllIndexesAsync();
        ArgumentNullException.ThrowIfNull(indexs);

        foreach (Meilisearch.Index index in indexs)
            await index.DeleteAsync();
    }
}
