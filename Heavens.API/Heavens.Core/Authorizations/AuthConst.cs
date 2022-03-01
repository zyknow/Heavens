using Bing.Extensions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Authorizations;

/// <summary>
/// 角色常量
/// </summary>
public class RoleConst
{
    /// <summary>
    /// 管理员
    /// </summary>
    public const string Admin = nameof(Admin);
    /// <summary>
    /// 用户
    /// </summary>
    public const string User = nameof(User);
}

/// <summary>
/// 策略常量
/// </summary>
public class PolicyConst
{
    public const string Users = nameof(Users);
}


/// <summary>
/// 策略授权静态类
/// </summary>
public class Policys
{
    /// <summary>
    /// 基于用户策略授权
    /// </summary>
    public static Policy Users { get; set; } = new Policy()
    {
        Name = nameof(Users),
        builder = (b) => b.RequireRole(RoleConst.User, RoleConst.Admin)
    };
}

/// <summary>
/// 策略授权类
/// </summary>
public class Policy
{
    public string Name { get; init; }
    public Action<AuthorizationPolicyBuilder> builder { get; init; }
}
