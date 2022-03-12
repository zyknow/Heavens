namespace Heavens.Application.AuthorizeApp.Services;

public interface IAuthorizeService
{
    /// <summary>
    /// 为用户创建token
    /// </summary>
    /// <param name="user"></param>
    /// <param name="expiredTime"></param>
    /// <returns></returns>
    string CreateToken(User user, long? expiredTime = null);
    Task<string> RefreshToken(string oldToken);
}
