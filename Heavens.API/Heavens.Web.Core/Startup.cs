using Furion;
using Furion.DependencyInjection;
using Heavens.Core.Authorizations;
using Heavens.Core.Extension.Cacheing;
using Heavens.EntityFramework.Core.DbContexts;
using Heavens.Web.Core.Filters;
using Heavens.Web.Core.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Heavens.Web.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddJwt<JwtHandler>();
        // 注册授权，必须在AddJwt后面注册！
        services.AddAuthorization(o => o.AddProjectPolicy());

        // 添加跨域
        services.AddCorsAccessor();

        services.AddControllers(o =>
        {
            var auditEnbale = App.GetConfig<bool>("AuditEnable");
            if (auditEnbale)
                // 审计过滤器
                o.Filters.Add(typeof(AuditActionFilter));
        })
                .AddInjectWithUnifyResult()
                .AddNewtonsoftJson(option =>
                {
                    //忽略循环引用
                    option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //设置时间格式
                    option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                })
                // 注册多语言
                .AddAppLocalization();

        // 注册远程 http get,post 请求
        services.AddRemoteRequest();

        // 注册定时任务
        //services.AddTaskScheduler();

        // 注册缓存
        services.AddCache();

        // 注册虚拟文件系统服务
        //services.AddVirtualFileServer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // 配置多语言，必须在 路由注册之前
        app.UseAppLocalization();

        //app.UseHttpsRedirection();

        // 启用EnableBuffering，否则Filter获取不到body
        app.Use(next => context =>
        {
            context.Request.EnableBuffering();
            return next(context);
        });

        app.UseStaticFiles();
        app.UseSerilogRequestLogging();    // 必须在 UseStaticFiles 和 UseRouting 之间
        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        if (env.IsDevelopment())
        {
            Scoped.Create((_, scope) =>
            {
                // 没创建Migration 就创建 Migration
                DefaultDbContext context = scope.ServiceProvider.GetRequiredService<DefaultDbContext>();
                context.Database.EnsureCreated();

                // 没迁移到数据库就迁移
                context.Database.Migrate();
            });
        }


        app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
