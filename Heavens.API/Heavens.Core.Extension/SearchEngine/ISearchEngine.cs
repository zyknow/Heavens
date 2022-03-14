using Meilisearch;

namespace Heavens.Core.Extension.SearchEngine;

public interface ISearchEngine
{
    MeilisearchClient Client { get; set; }

    Task AddOrUpdate<T>(string indexStr, T entity = null) where T : class, new();
    Task AddOrUpdates<T>(string indexStr, List<T> list = null) where T : class, new();
    Task<bool> Connect();
    Task DeleteAllIndex();
    Task DeleteDocuments(string indexStr, string[] ids);
    Task DeleteIndex(string indexStr);
    Task<bool> IsHealthy();
    Task<SearchResult<T>> Search<T>(string indexStr, string search, SearchQuery searchQuery = null);
}
