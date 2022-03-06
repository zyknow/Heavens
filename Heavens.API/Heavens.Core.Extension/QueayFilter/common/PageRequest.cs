using Furion.UnifyResult;
using Furion;
using Heavens.Core.Extension.QueayFilter.helper;
using System.Linq.Expressions;
using Microsoft.Extensions.Hosting;
using Heavens.Core.Extension.Extensions;

namespace Heavens.Core.Extension.QueayFilter.common;

/// <summary>
/// 
/// </summary>
public class PageRequest : BaseRequest
{
    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; } = 1;
    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; } = 50;
}

