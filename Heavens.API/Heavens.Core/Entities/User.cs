
using Furion.DatabaseAccessor;
using Furion.DataEncryption;
using Heavens.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heavens.Core.Entities;

/// <summary>
/// 用户信息表
/// </summary>
public class User : BaseEntity, IEntityTypeBuilder<User>, IEntitySeedData<User>
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
    /// PIN码
    /// </summary>
    public int PIN { get; set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
    /// <summary>
    /// 持有的角色 |号分割
    /// </summary>
    public string Roles { get; set; }
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

    public void Configure(EntityTypeBuilder<User> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {

        // 登录账号必填
        entityBuilder.Property(e => e.Account).IsRequired();


    }

    public IEnumerable<User> HasData(DbContext dbContext, Type dbContextLocator)
    {
        List<User> users = new List<User>
            {
                new User()
                {
                    Id = 1,
                    Name = "管理员",
                    Enabled = true,
                    Account = "admin",
                    Passwd = MD5Encryption.Encrypt("123456"),
                    Roles = "Admin|Test",
                },
                new User()
                {
                    Id = 2,
                    Name = "Test",
                    Enabled = true,
                    Account = "Test",
                    Passwd = MD5Encryption.Encrypt("123456"),
                },
            };

        return users;
    }
}
