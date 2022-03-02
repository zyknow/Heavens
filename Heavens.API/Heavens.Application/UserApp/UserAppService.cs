using Heavens.Application.UserApp.Dtos;

namespace Heavens.Application.UserApp;

/// <summary>
/// 用户接口
/// </summary>
[Authorize]
public class UserAppService : BaseAppService<User, UserDto>
{

    public UserAppService(IRepository<User> userRepository) : base(userRepository)
    {
    }



    /// <summary>
    /// 获取用户信息，请求头中带token
    /// </summary>
    /// <returns></returns>
    public async Task<UserDto> GetByToken()
    {
        int userId = TokenInfo.Id;

        var roles = TokenInfo.Roles;

        if (userId.IsZeroOrMinus())
        {
            throw Oops.Oh(Excode.DATA_NO_EXSITS);
        }

        User user = await _repository
                .AsQueryable()
                .FirstOrDefaultAsync(r => r.Enabled && r.Id == userId);
        return user.Adapt<UserDto>();
    }

    /// <summary>
    /// 用户修改密码
    /// </summary>
    /// <param name="oldPasswd">旧密码</param>
    /// <param name="newPasswd">新密码</param>
    /// <returns></returns>
    public async Task<bool> UpdatePasswd([Required] string oldPasswd, [Required] string newPasswd)
    {
        User user = await _repository.FirstAsync(u => u.Id == TokenInfo.Id, true);

        // 不存在用户
        if (user.IsNull())
        {
            throw Oops.Oh(Excode.DATA_NO_EXSITS);
        }

        // 旧密码错误 
        if (user.Passwd != MD5Encryption.Encrypt(oldPasswd))
        {
            throw Oops.Oh(Excode.USER_OLD_PASSWD_ERROR);
        }

        user.Passwd = MD5Encryption.Encrypt(newPasswd);

        return await _repository.SaveNowAsync() > 0;
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public override async Task<UserDto> Add(UserDto dto)
    {
        // 登录名重复
        if (await _repository.AnyAsync(u => u.Account == dto.Account))
        {
            throw Oops.Oh(Excode.USER_NAME_REPEAT);
        }

        // 密码加密
        dto.Passwd = MD5Encryption.Encrypt(dto.Passwd);

        //if (!dto.Birth.IsNull())
        //    dto.Age = (int)(DateTime.Now - dto.Birth)?.TotalDays / 365;

        UserDto user = await base.Add(dto);

        // 添加用户失败
        if (user.IsNull())
        {
            throw Oops.Oh(Excode.ADD_DATA_FAILD);
        }

        return user;
    }

    /// <summary>
    /// 编辑用户
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public override async Task<UserDto> Update(UserDto dto)
    {
        User user = await _repository.Where(u => u.Id == dto.Id).FirstOrDefaultAsync();

        User upUser = dto.Adapt<User>();


        if (user.IsNull())
        {
            throw Oops.Oh(Excode.DATA_NO_EXSITS);
        }

        if (upUser.Passwd.IsEmpty())
        {
            upUser.Passwd = user.Passwd;
        }
        else
        {
            upUser.Passwd = MD5Encryption.Encrypt(upUser.Passwd);
        }

        User updatedUser = (await _repository.UpdateNowAsync(upUser)).Entity;

        return updatedUser.Adapt<UserDto>();
    }
}
