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

    public string version { get; set; }

    /// <summary>
    /// 获取后台版本号
    /// </summary>
    /// <returns></returns>
    public string GetVersion()
    {
        if (version.IsEmpty())
        {
            Version v = Assembly.GetEntryAssembly().GetFileVersion();
            version = v.ToString();
        }
        return version;
    }
}
