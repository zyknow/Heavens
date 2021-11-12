using Heavens.Core.Entities.Base;
using System;
using System.Collections.Generic;

namespace Heavens.Application.UserApp.Dtos;

/// <summary>
/// 用户信息表
/// </summary>
public class UserDto : BaseEntityProp
{
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 登录账号
    /// </summary>
    public string Account { get; set; }
    /// <summary>
    /// 登录密码
    /// </summary>
    public string Passwd { get; set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
    /// <summary>
    /// 持有的角色 |号分割
    /// </summary>
    public List<string> Roles { get; set; }
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public bool Sex { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///最后登录时间 
    /// </summary>
    public DateTime LastLoginTime { get; set; } = DateTime.Now;

}
