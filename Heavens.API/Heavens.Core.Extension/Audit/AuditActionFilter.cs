using Bing.Utils.IdGenerators.Core;
using Furion;
using Furion.ClayObject.Extensions;
using Furion.DatabaseAccessor;
using Furion.JsonSerialization;
using Heavens.Core.Authorizations;
using Heavens.Core.Entities;
using Meilisearch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.Audit;

/// <summary>
/// 审计 Filter
/// </summary>
public class AuditActionFilter : IAsyncActionFilter
{
    public AuditActionFilter(IRepository<Entities.Audit> auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public IRepository<Entities.Audit> _auditRepository { get; }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 拦截之前
        IgnoreAuditAttribute ignoreAudit = context.HttpContext.GetMetadata<IgnoreAuditAttribute>();
        if (ignoreAudit != null) { await next(); return; }


        var watch = Stopwatch.StartNew();

        var result = await next();

        watch.Stop();

        // 请求用户所持有的角色
        var roles = TokenInfo.Roles;
        var rolesStr = !roles.IsEmpty() ? string.Join("|", roles) : "";

        // 请求的控制器
        var type = (context.ActionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo.AsType();
        var serverName = type != null ? type.FullName : "";

        //请求的方法
        var method = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;

        // 异常拦截
        var exception = string.Empty;

        // 返回数据
        var returnValue = string.Empty;
        if (result != null)
        {
            switch (result.Result)
            {
                case ObjectResult objectResult:
                    if (objectResult.Value != null)
                        returnValue = JsonConvert.SerializeObject(objectResult.Value);
                    break;

                case JsonResult jsonResult:
                    if (jsonResult.Value != null)
                    {
                        var dic = jsonResult.Value.ToDictionary();
                        if (dic.ContainsKey("Errors") && dic["Errors"] != null)
                            exception = JsonConvert.SerializeObject(dic["Errors"]);
                        returnValue = JsonConvert.SerializeObject(jsonResult.Value);
                    }
                    break;

                case ContentResult contentResult:
                        returnValue = contentResult.Content;
                    break;
            }
        }
        if (result?.Exception != null)
        {
            exception = result.Exception.ToString();
        }

        // body参数
        var body = await ReadBodyAsync(context.HttpContext.Request);

        // query参数
        var query = context.HttpContext.Request.QueryString.Value;

        // path参数
        var path = context.HttpContext.Request.Path;

        var audit = new Entities.Audit()
        {
            UserRoles = rolesStr,
            ServiceName = serverName,
            ExecutionMs = (int)watch.ElapsedMilliseconds,
            MethodName = method?.Name,
            ReturnValue = returnValue,
            ClientIpAddress = context.HttpContext.GetRemoteIpAddressToIPv4(),
            HttpMethod = context.HttpContext.Request.Method,
            Exception = exception,
            Body = body,
            Query = query,
            Path = path,
        };
        audit.SetCreateByHttpToken();

#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        await _auditRepository.InsertAsync(audit);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
    }

    #region 私有方法
    /// <summary>
    /// 获取编码
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private Encoding GetRequestEncoding(HttpRequest request)
    {
        var requestContentType = request.ContentType;
        var requestMediaType = requestContentType == null ? default : new MediaType(requestContentType);
        var requestEncoding = requestMediaType.Encoding;
        if (requestEncoding == null)
        {
            requestEncoding = Encoding.UTF8;
        }
        return requestEncoding;
    }
    /// <summary>
    /// 读取body
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<string> ReadBodyAsync(HttpRequest request)
    {
        string result = string.Empty;
        request.EnableBuffering();
        request.Body.Position = 0;
        var stream = request.Body;
        long? length = request.ContentLength;
        if (length != null && length > 0)
        {
            StreamReader streamReader = new StreamReader(stream, GetRequestEncoding(request));
            result = await streamReader.ReadToEndAsync();
        }
        request.Body.Position = 0;
        return result;
    }
    #endregion
}
