using Heavens.Core.Entities.Base;


namespace Heavens.Application.AuditApp.Dtos;
/// <summary>
/// Audit Dto
/// </summary>
public class AuditDto : BaseEntityProp
{
    /// <summary>
    /// 用户持有角色
    /// </summary>
    public string UserRoles { get; set; }
    /// <summary>
    /// 服务 (类/接口) 名
    /// </summary>
    public string ServiceName { get; set; }
    /// <summary>
    /// 执行方法名称
    /// </summary>
    public string MethodName { get; set; }
    /// <summary>
    /// 请求路径
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// Body参数
    /// </summary>
    public string Body { get; set; }
    /// <summary>
    /// Query参数
    /// </summary>
    public string Query { get; set; }
    /// <summary>
    /// Http请求方法
    /// </summary>
    public string HttpMethod { get; set; }
    /// <summary>
    /// 返回值
    /// </summary>
    public string ReturnValue { get; set; }
    /// <summary>
    /// 方法调用的总持续时间（毫秒）
    /// </summary>
    public int ExecutionMs { get; set; }
    /// <summary>
    /// 客户端的IP地址
    /// </summary>
    public string ClientIpAddress { get; set; }
    /// <summary>
    /// 方法执行期间发生异常
    /// </summary>
    public string Exception { get; set; }
}
