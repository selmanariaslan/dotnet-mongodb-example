using Arch.CoreLibrary.Managers;
using Arch.CoreLibrary.Repositories;
using Arch.Data;
using Arch.Mongo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Arch.Services.Bootstrappers
{
    public static class Ioc
    {
        public static void RegisterScopes(IServiceCollection services)
        {
            addDbContexts(services);
            addDependencies(services);
        }
        private static void addDependencies(IServiceCollection services)
        {
            services.AddScoped<IGenericRepository<LogManagementContext>, GenericRepository<LogManagementContext>>();

            services.AddScoped<IMongoDbSettings>(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            services.AddScoped(typeof(Mongo.Managers.IGenericRepository<>), typeof(Mongo.Managers.GenericRepository<>));
            services.AddScoped(typeof(Mongo.Managers.IServiceManager), typeof(Mongo.Managers.ServiceManager));
            services.AddScoped(typeof(IMemoryCache), typeof(MemoryCache));
            services.AddScoped<IServiceManager, ServiceManager>();
        }

        private static void addDbContexts(IServiceCollection services)
        {
            //var mongoConn = builder.Configuration.GetSection("BookStore");
            //services.Configure<MongoDbSettings>();

            var logManagementCnn = DatabaseConfig.GetConnectionString("LogManagement");
            services.AddDbContext<LogManagementContext>(options => options.UseSqlServer(logManagementCnn));
        }
    }
}
