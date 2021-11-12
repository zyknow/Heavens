﻿using Furion;
using System.IdentityModel.Tokens.Jwt;

namespace Heavens.Core.Authorizations;

/// <summary>
/// httpcontext 中 token 包含的信息
/// </summary>
public class TokenInfo
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public static int Id { get => (App.User?.FindFirst(JwtRegisteredClaimNames.Sid)?.Value).ToInt(); private set { } }
    /// <summary>
    /// 用户名
    /// </summary>
    public static string Account { get => App.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value; private set { } }

}
