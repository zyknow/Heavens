using Meilisearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.SearchEngine;
public class SearchRequest : SearchQuery
{
    public string Search { get; set; }
}
