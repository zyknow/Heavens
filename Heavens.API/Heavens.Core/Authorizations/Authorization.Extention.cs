using CSRedis.Internal.ObjectPool;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Heavens.Core.Authorizations;

public static class Authorization
{

    /// <summary>
    /// 添加该项目的策略授权
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static AuthorizationOptions AddProjectPolicy(this AuthorizationOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        // 反射添加策略
        var policyProps = typeof(Policys).GetProperties();
        var policyObj = new Policys();
        foreach (var policyType in policyProps)
        {
            var policy = policyType.GetPropertyValue(policyObj) as Policy;
            options.AddPolicy(policy.Name, policy.builder);
        }
        return options;
    }
}
