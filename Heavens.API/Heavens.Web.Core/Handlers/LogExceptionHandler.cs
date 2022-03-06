using Bing.Expressions;
using Furion.ClayObject.Extensions;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Web.Core.Handlers;
public class LogExceptionHandler : IGlobalExceptionHandler, ISingleton
{
    public LogExceptionHandler(ILogger<LogExceptionHandler> logger)
    {
        _logger = logger;
    }

    public ILogger<LogExceptionHandler> _logger { get; }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        dynamic exp = context.Exception;

        var dic = context.ToDictionary();

        #region 写日志
        if (dic.ContainsKey("ErrorCode"))
            _logger.LogWarning(context.Exception, context.Exception.Message);
        else
            _logger.LogError(context.Exception, context.Exception.Message);
        #endregion


        return Task.CompletedTask;
    }
}
