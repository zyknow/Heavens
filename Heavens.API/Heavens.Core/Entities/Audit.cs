using Furion.DatabaseAccessor;
using Heavens.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Entities;

/// <summary>
/// 审计
/// </summary>
public class Audit : BaseEntity, IEntityTypeBuilder<Audit>
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

    public void Configure(EntityTypeBuilder<Audit> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
    }
}

