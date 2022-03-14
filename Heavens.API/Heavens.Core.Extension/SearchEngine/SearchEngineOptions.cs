using Furion.ConfigurableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.SearchEngine;
/// <summary>
/// 缓存设置选项
/// </summary>
public class SearchEngineOptions : IConfigurableOptions
{
    public bool Enabled { get; set; }

    public string ConnectStr { get; set; }

    public string MasterKey { get; set; }

}
