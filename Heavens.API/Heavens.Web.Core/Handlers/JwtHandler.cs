using Furion.Authorization;
using Furion.DatabaseAccessor;
using Furion.DataEncryption;
using Heavens.Application.AuthorizeApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Heavens.Web.Core.Handlers;

public class JwtHandler : AppAuthorizeHandler
{
    /// <summary>
    /// 重写 Handler 添加自动刷新收取逻辑
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task HandleAsync(AuthorizationHandlerContext context)
    {
        Microsoft.AspNetCore.Http.DefaultHttpContext currentContext = context.GetCurrentHttpContext();
        // 刷新token
        string refToken = currentContext.Request.Headers["X-Authorization"].SafeString().Replace("Bearer ", "");
        // 携带token
        string token = currentContext.Request.Headers["Authorization"].SafeString().Replace("Bearer ", "");

        // token已失效 且 携带了刷新token
        if (!JWTEncryption.Validate(token).IsValid && !refToken.IsEmpty())
        {
            // 先刷新Token，否则 TokenInfo ( App.User )在token失效下无法使用
            JWTEncryption.AutoRefreshToken(context, currentContext);

            // 验证刷新token有效性
            if (!JWTEncryption.Validate(refToken).IsValid)
            {
                context.Fail();    // 刷新token无效
                return;
            }

            // 验证刷新token有效性
            if (!JWTEncryption.Validate(refToken).IsValid)
            {
                context.Fail();    // 刷新token无效
                return;
            }

            IAuthorizeService authorizeServices = currentContext.RequestServices.GetService<IAuthorizeService>();
            IRepository<User> userRepository = currentContext.RequestServices.GetService<IRepository<User>>();

            JsonWebToken tokenInfo = JWTEncryption.ReadJwtToken(token);

            int userId = tokenInfo.Claims.First(c => c.Type == ClaimTypes.Sid)?.Value?.ToInt() ?? 0;

            if(userId == 0)
            {
                context.Fail();    // 授权失败
                return;
            }

            User user = await userRepository.FirstOrDefaultAsync(c => c.Id == userId);

            if (user == null)
            {
                context.Fail();    // 授权失败
                return;
            }
        }
        await base.HandleAsync(context);
    }
}
