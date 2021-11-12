using Heavens.Core.Entities;
using System.Threading.Tasks;

namespace Heavens.Application.AuthorizeApp.Services;

public interface IAuthorizeServices
{
    /// <summary>
    /// 为用户创建token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    string CreateToken(User user, long? expiredTime = null);
    Task<string> RefreshToken(string oldToken);
}
