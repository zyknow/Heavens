using Meilisearch;

namespace Heavens.Core.Extension.SearchEngine;

public interface ISearchEngine
{

    Task AddOrUpdate<T>(string indexStr, T entity = null) where T : class, new();
    Task AddOrUpdates<T>(string indexStr, List<T> list = null) where T : class, new();
    Task<bool> Connect(string connectStr = null, string key = null);
    Task DeleteAllIndex();
    Task DeleteDocuments(string indexStr, string[] ids);
    Task DeleteIndex(string indexStr);
    Task<bool> IsHealthy();
    Task<SearchResult<T>> Search<T>(string indexStr, string search);
}
