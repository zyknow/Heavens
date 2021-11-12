using Furion;
using Heavens.EntityFramework.Core.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Heavens.EntityFramework.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseAccessor(options =>
            {
                //options.AddDbPool<DefaultDbContext>();

                options.AddDbPool<DefaultDbContext>(default, opt =>
             {
                 //opt.UseBatchEF_MySQLPomelo();
                    opt.UseBatchEF_Sqlite();
             });
            }, "Heavens.Database.Migrations");
        }
    }
}