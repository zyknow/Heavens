using Heavens.Application.AuditApp.Services;
using Heavens.Application.AuditApp.Dtos;
namespace Heavens.Application.AuditApp;

/// <summary>
/// 审计接口
/// </summary>
[Authorize]
public class AuditAppService : BaseAppService<Audit, AuditDto>
{
    public IAuditService _auditService { get; set; }
    public AuditAppService(IRepository<Audit> auditRepository,IAuditService auditService) : base(auditRepository)
    {
        _auditService = auditService;
    }
    
    
}
