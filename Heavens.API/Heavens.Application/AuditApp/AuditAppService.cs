using Heavens.Application.AuditApp.Services;
using Heavens.Application.AuditApp.Dtos;
using Microsoft.AspNetCore.Mvc;
using Heavens.Core.Extension.Audit;

namespace Heavens.Application.AuditApp;

/// <summary>
/// 审计接口
/// </summary>
[Authorize,IgnoreAudit]
public class AuditAppService : BaseAppService<Audit, AuditDto>
{
    public IAuditService _auditService { get; set; }
    public AuditAppService(IRepository<Audit> auditRepository, IAuditService auditService) : base(auditRepository)
    {
        _auditService = auditService;
    }

    [HttpPost]
    public async Task<PagedList<AuditPage>> Page(PageRequest request)
    {
        var data = await _repository
            .Where(request.GetRulesExpression<Audit>())
            .SortBy(request.Sort)
            .Select(x => x.Adapt<AuditPage>())
            .ToPagedListAsync(request.Page, request.PageSize);

        return data;
    }


}
