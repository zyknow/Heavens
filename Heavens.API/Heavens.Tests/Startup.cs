using Furion;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("Heavens.Tests.Startup", "Heavens.Tests")]
namespace Heavens.Tests;

/// <summary>
/// 单元测试启动类
/// </summary>
/// <remarks>在这里可以使用 Furion 几乎所有功能</remarks>
public sealed class Startup : XunitTestFramework
{
    public Startup(IMessageSink messageSink) : base(messageSink)
    {
        // 初始化 IServiceCollection 对象
        IServiceCollection services = Inject.Create();

        // 在这里可以和 .NET Core 一样注册服务了！！！！！！！！！！！！！！

        // 构建 ServiceProvider 对象
        services.Build();
    }
}
