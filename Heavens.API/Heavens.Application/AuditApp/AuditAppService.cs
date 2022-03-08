using Heavens.Application.AuditApp.Services;
using Heavens.Application.AuditApp.Dtos;
using Microsoft.AspNetCore.Mvc;
using Heavens.Core.Extension.Audit;
using Furion.UnifyResult;
using Furion;
using System.Linq.Expressions;
using Heavens.Core.Extension.Extensions;
using Heavens.Core.Extension.QueayFilter.common;
using Heavens.Core.Extension.QueayFilter;
using Bing.Expressions;

namespace Heavens.Application.AuditApp;

/// <summary>
/// 审计接口
/// </summary>
[Authorize, IgnoreAudit]
public class AuditAppService : BaseAppService<int, Audit, AuditDto, AuditPage>
{
    public IAuditService _auditService { get; set; }
    public AuditAppService(IRepository<Audit> auditRepository, IAuditService auditService) : base(auditRepository)
    {
        _auditService = auditService;

        _queryAcions = new List<IQueryAction<Audit>>()
        {
            new QueryAction<Audit, double, bool>(nameof(AuditPage.HasBody),p=> p.Body.Length,p=> !string.IsNullOrEmpty(p.Body)),
            new QueryAction<Audit, double, bool>(nameof(AuditPage.HasQuery),p=> p.Query.Length,p=> !string.IsNullOrEmpty(p.Query)),
            new QueryAction<Audit, double, bool>(nameof(AuditPage.HasException),p=> p.Exception.Length,p=> !string.IsNullOrEmpty(p.Exception)),
            new QueryAction<Audit, double, bool>(nameof(AuditPage.HasReturnValue),p=> p.ReturnValue.Length,p=> !string.IsNullOrEmpty(p.ReturnValue)),
        };
    }

    #region 禁用接口

    [NonAction]
    public override Task<AuditDto> Add(AuditDto modelDto)
    {
        return base.Add(modelDto);
    }
    [NonAction]
    public override Task DeleteById([Required] int id)
    {
        return base.DeleteById(id);
    }
    [NonAction]
    public override Task<int> DeleteByIds([FromBody] int[] ids)
    {
        return base.DeleteByIds(ids);
    }
    [NonAction]
    public override Task<AuditDto> Update(AuditDto modelDto)
    {
        return base.Update(modelDto);
    }

    #endregion
}
