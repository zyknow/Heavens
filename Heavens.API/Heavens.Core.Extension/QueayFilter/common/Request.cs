using Heavens.Core.Extension.QueayFilter.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.QueayFilter.common;
public class Request : BaseRequest
{
    /// <summary>
    /// 数据限制
    /// </summary>
    public int Limit { get; set; }
}
