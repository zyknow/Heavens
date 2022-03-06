using Bing.Date;
using Serilog;
using Serilog.Events;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args).Inject();


#region Log 配置
builder.Host.UseSerilogDefault(config =>//默认集成了 控制台 和 文件 方式。如需自定义写入，则传入需要写入的介质即可：
{
    string date = DateTime.Now.ToString("yyyy-MM-dd");//按时间创建文件夹
    string outputTemplate = "{NewLine}【{Level:u3}】{Timestamp:yyyy-MM-dd HH:mm:ss.fff}" +
    "{NewLine}#Msg#{Message:lj}" +
    "{NewLine}#Pro #{Properties:j}" +
    "{NewLine}#Exc#{Exception}" +
    new string('-', 50) + "{NewLine}";//输出模板

    ///1.输出所有restrictedToMinimumLevel：LogEventLevel类型
    config
        //.MinimumLevel.Debug() // 所有Sink的最小记录级别
        //.MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
        //.Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: outputTemplate)
        .WriteTo.File($"log/{LogEventLevel.Information}/{date}.log",
               outputTemplate: outputTemplate,
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollOnFileSizeLimit: true,          // 限制单个文件的最大长度
                retainedFileCountLimit: 50,         // 最大保存文件数,等于null时永远保留文件。
                fileSizeLimitBytes: 1024 * 1024,      // 最大单个文件大小
                encoding: Encoding.UTF8            // 文件字符编码
            )

    #region 2.按LogEventLevel.输出独立发布/单文件

        // Debug 
        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Debug)//筛选过滤
            .WriteTo.File($"log/{LogEventLevel.Debug}/{date}.log",
                outputTemplate: outputTemplate,
                encoding: Encoding.UTF8            // 文件字符编码
             )
        )
        // Warning 
        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning)//筛选过滤
            .WriteTo.File($"log/{LogEventLevel.Warning}/{date}.log",
                outputTemplate: outputTemplate,
                encoding: Encoding.UTF8            // 文件字符编码
             )
        )
        // Error 
        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error)//筛选过滤
            .WriteTo.File($"log/{LogEventLevel.Error}/{date}.log",
                outputTemplate: outputTemplate,
                encoding: Encoding.UTF8            // 文件字符编码
             )
        );

    #endregion 按LogEventLevel 独立发布/单文件


});
#endregion

var app = builder.Build();
app.Run();
