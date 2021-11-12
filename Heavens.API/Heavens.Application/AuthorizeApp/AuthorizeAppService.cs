
using Bing.Helpers;
using Furion.DatabaseAccessor;
using Furion.DataEncryption;
using Furion.DynamicApiController;
using Furion.FriendlyException;
using Heavens.Application.AuthorizeApp.Dtos;
using Heavens.Application.AuthorizeApp.Services;
using Heavens.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Heavens.Application.AuthorizeApp;

/// <summary>
/// 权限认证接口
/// </summary>
[AllowAnonymous]
[ApiDescriptionSettings(Order = 100)]
public class AuthorizeAppService : IDynamicApiController
{
    public AuthorizeAppService(IRepository<User> userRepository, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, IAuthorizeServices userAuthorizeServices)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _env = env;
        _userAuthorizeServicese = userAuthorizeServices;
    }

    public IRepository<User> _userRepository { get; }
    public IHttpContextAccessor _httpContextAccessor { get; }
    public IWebHostEnvironment _env { get; }
    public IAuthorizeServices _userAuthorizeServicese { get; }

    /// <summary>
    /// 获取Token
    /// </summary>
    /// <param name="para">请求参数</param>
    /// <returns> 返回jwt token </returns>
    [HttpPost]
    public async Task GetToken(LoginInput para)
    {
        User user = null;

        if (para.LoginType == LoginType.AccountPassword)
        {
            if (Valid.IsEmail(para.Account)) // 邮箱
            {
                user = await _userRepository.FirstOrDefaultAsync(u => u.Email == para.Account && u.Enabled) ?? throw Oops.Oh(Excode.USER_NAME_OR_PASSWD_ERROR);
            }
            else
            {
                user = await _userRepository.FirstOrDefaultAsync(u => u.Account == para.Account && u.Enabled) ?? throw Oops.Oh(Excode.USER_NAME_OR_PASSWD_ERROR);
            }
        }
        else if (para.LoginType == LoginType.PhonePasswd)
        {
            user = await _userRepository.FirstOrDefaultAsync(u => u.Phone == para.Account && u.Enabled) ?? throw Oops.Bah(Excode.USER_NAME_OR_PASSWD_ERROR);
        }

        // 是否被禁用
        if (!user.Enabled)
        {
            throw Oops.Oh(Excode.USER_DISABLED);
        }

        //密码是否正确
        if (MD5Encryption.Encrypt(para.Passwd) != user.Passwd)
        {
            throw Oops.Oh(Excode.USER_NAME_OR_PASSWD_ERROR);
        }

        //string accountToken = _userAuthorizeServicese.CreateToken(user, para.KeepAlive ? 60 * 24 * 15 : null);
        string accountToken = _userAuthorizeServicese.CreateToken(user, para.KeepAlive ? 10080 : null);
        string refreshToken = JWTEncryption.GenerateRefreshToken(accountToken);

        _httpContextAccessor.HttpContext.Response.Headers["access-token"] = accountToken;
        _httpContextAccessor.HttpContext.Response.Headers["x-access-token"] = refreshToken;

        // 设置 Swagger 自动登录
        _httpContextAccessor.HttpContext.SigninToSwagger(accountToken);
    }

    /// <summary>
    /// 刷新token
    /// </summary>
    /// <param name="oldToken"></param>
    /// <returns></returns>
    [Authorize]
    public string RefreshToken(string oldToken)
    {
        return JWTEncryption.GenerateRefreshToken(oldToken, 43200);
        // 验证当前用户
        //return _userAuthorizeServicese.RefreshToken(oldToken);
    }
}
