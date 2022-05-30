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
            //services.AddScoped<IGenericRepository<AlonetContext>, GenericRepository<AlonetContext>>();
            //services.AddScoped<IGenericRepository<AlonetCommonContext>, GenericRepository<AlonetCommonContext>>();
            //services.AddScoped<IGenericRepository<AlonetLogManagementContext>, GenericRepository<AlonetLogManagementContext>>();
            //services.AddScoped<IGenericRepository<AlonetContactManagementContext>, GenericRepository<AlonetContactManagementContext>>();
            //services.AddScoped<IGenericRepository<AlonetTicketManagementContext>, GenericRepository<AlonetTicketManagementContext>>();

            //services.AddScoped<IServiceManager, ServiceManager>();
        }

        private static void addDbContexts(IServiceCollection services)
        {
            //var BookStoreCnn = services.Configure <List<MongoDbSettings>>()

            //    .Build();


            //services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
            //services.AddSingleton<IMongoDbSettings>(serviceProvider =>
            //    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);




            //var alonetCnn = DatabaseConfig.GetConnectionString("Alonet");
            //services.AddDbContext<AlonetContext>(options => options.UseSqlServer(alonetCnn));

            //var alonetCommonCnn = DatabaseConfig.GetConnectionString("AlonetCommon");
            //services.AddDbContext<AlonetCommonContext>(options => options.UseSqlServer(alonetCommonCnn));

            //var alonetLogManagementCnn = DatabaseConfig.GetConnectionString("AlonetLogManagement");
            //services.AddDbContext<AlonetLogManagementContext>(options => options.UseSqlServer(alonetLogManagementCnn));

            //var alonetContactManagementCnn = DatabaseConfig.GetConnectionString("AlonetContactManagement");
            //services.AddDbContext<AlonetContactManagementContext>(options => options.UseSqlServer(alonetContactManagementCnn));

            //var alonetTicketManagementCnn = DatabaseConfig.GetConnectionString("AlonetTicketManagement");
            //services.AddDbContext<AlonetTicketManagementContext>(options => options.UseSqlServer(alonetTicketManagementCnn));
        }
    }
}
