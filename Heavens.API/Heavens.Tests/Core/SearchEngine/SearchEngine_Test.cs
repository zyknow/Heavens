using Furion;
using Heavens.Core.Extension.SearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Heavens.Tests.Core.SearchEngine;
public class SearchEngine_Test
{
    ISearchEngine _searchEngine { get; set; }

    string indexName { get; set; } = nameof(SearchEngine_Test);

    public SearchEngine_Test()
    {
        _searchEngine = App.GetService<ISearchEngine>();
    }

    [Fact]
    public async Task Connect_Test()
    {
        var res = _searchEngine.Connect();
        Assert.True(res);
    }

    [Fact]
    public async Task AddOrUpdate_Test()
    {
        _searchEngine.Connect();
        await _searchEngine.AddOrUpdate(indexName, new SE_Student()
        {
            Name = "1"
        });
    }
}
