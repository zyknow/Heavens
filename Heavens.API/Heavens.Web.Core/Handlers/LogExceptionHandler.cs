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
public class LogExceptionHandler : IGlobalExceptionHandler,ISingleton
{
    public LogExceptionHandler(ILogger<LogExceptionHandler> logger)
    {
        _logger = logger;
    }

    public ILogger<LogExceptionHandler> _logger { get; }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        // 写日志
        _logger.LogError(context.Exception, context.Exception.Message);

        return Task.CompletedTask;
    }
}
