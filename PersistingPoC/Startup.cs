using AutoMapper;
using Crushbank.Core.Interfaces;
using Crushbank.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PersistingPoC.Integrations;
using PersistingPoC.Interfaces;
using PersistingPoC.Repository.Models.Mongodb;
using PersistingPoC.Repository.Models.Sql;
using PersistingPoC.Service.Dtos;
using PersistingPoC.Service.Interfaces;
using PersistingPoC.Service.Services;
using PersistingPoC.Service.Services.Redis;
using Enumerations = PersistingPoC.Entities.Enums;
using RepositoryInterfacesMongodb = PersistingPoC.Repository.Interfaces.Mongodb;
using RepositoryInterfacesSql = PersistingPoC.Repository.Interfaces.Sql;
using RepositoryRepositoriesMongodb = PersistingPoC.Repository.Repositories.Mongodb;
using RepositoryRepositoriesPostgreSql = PersistingPoC.Repository.Repositories.PostgreSql;
using RepositoryRepositoriesSql = PersistingPoC.Repository.Repositories.Sql;
using ServiceInterfacesConnectWise = PersistingPoC.Service.Interfaces.ConnectWise;
using ServiceInterfacesSql = PersistingPoC.Service.Interfaces.Sql;
using ServiceServicesConnectWise = PersistingPoC.Service.Services.ConnectWise;
using ServiceServicesMongodb = PersistingPoC.Service.Services.Mongodb;
using ServiceServicesSql = PersistingPoC.Service.Services.Sql;

namespace PersistingPoC
{
    public class Startup
    {
        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureStorage(services);
            services.AddDbContext<SqlServerDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            services.AddTransient<IConnectWiseTicketIntegration, ConnectWiseTicketIntegration>();

            services.AddTransient<IIntegrationRestService, IntegrationRestService>();
            services.AddTransient<IBearerTokenRestService, BearerTokenRestService>();

            services.AddTransient<IWatsonContentService, WatsonContentService>();

            services.AddTransient<ServiceInterfacesConnectWise.ITicketService, ServiceServicesConnectWise.TicketService>();
            services.AddTransient<ServiceInterfacesConnectWise.IUtilityService, ServiceServicesConnectWise.UtilityService>();

            services.AddTransient<ServiceInterfacesSql.ITaskProcessService, ServiceServicesSql.TaskProcessService>();
            services.AddTransient<ServiceInterfacesSql.ITaskTypeIntegrationService, ServiceServicesSql.TaskTypeIntegrationService>();

            services.AddTransient<RepositoryInterfacesSql.ITaskProcessRepository, RepositoryRepositoriesSql.TaskProcessRepository>();
            services.AddTransient<RepositoryInterfacesSql.ITaskTypeIntegrationRepository, RepositoryRepositoriesSql.TaskTypeIntegrationRepository>();

            services.AddAutoMapper(typeof(CompanyDtoProfile), typeof(CompanyIntegrationDtoProfile), typeof(IntegrationTypeDtoProfile),
                typeof(TaskProcessDtoProfile), typeof(TaskTypeDtoProfile), typeof(TaskTypeIntegrationDtoProfile), typeof(TicketDtoProfile),
                typeof(TicketDetailDtoProfile));
            
            services.AddSignalR();
            services.AddHostedService<Worker>();

            services.AddTransient<IRedisService, RedisService>();

            services.AddEasyCaching(options =>
            {
                options.UseRedis(redisConfig =>
                {
                    redisConfig.DBConfig.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint("localhost", 6379));
                    redisConfig.DBConfig.Password = "123456";

                    redisConfig.DBConfig.AllowAdmin = true;
                },
                "redis1");
            });
        }

        private void ConfigureStorage(IServiceCollection services)
        {
            var storageType = _configuration.GetValue<int>("IntegrationConfiguration:StoreType");

            switch (storageType)
            {
                case (int)Enumerations.StorageTypes.SqlServer:
                    services.AddTransient<ITicketService, ServiceServicesSql.TicketService>();
                    services.AddTransient<ServiceInterfacesSql.ITicketDetailService, ServiceServicesSql.TicketDetailService>();
                    
                    services.AddTransient<RepositoryInterfacesSql.ITicketDetailRepository, RepositoryRepositoriesSql.TicketDetailRepository>();
                    services.AddTransient<RepositoryInterfacesSql.ITicketRepository, RepositoryRepositoriesSql.TicketRepository>();
                    break;
                case (int)Enumerations.StorageTypes.MongoDb:
                    services.Configure<TicketStoreDatabaseSettings>(_configuration.GetSection("TicketStoreDatabaseSettings"));
                    services.AddSingleton<RepositoryInterfacesMongodb.ITicketStoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<TicketStoreDatabaseSettings>>().Value);
                    
                    services.AddTransient<ITicketService, ServiceServicesMongodb.TicketService>();
                    
                    services.AddTransient<RepositoryInterfacesMongodb.ITicketRepository, RepositoryRepositoriesMongodb.TicketRepository>();
                    break;
                case (int)Enumerations.StorageTypes.PostgreSql:
                    services.AddEntityFrameworkNpgsql().AddDbContext<PostgreSqlDbContext>(opt => opt.UseNpgsql(_configuration.GetConnectionString("TicketsConnection")));

                    services.AddTransient<ITicketService, ServiceServicesSql.TicketService>();
                    services.AddTransient<ServiceInterfacesSql.ITicketDetailService, ServiceServicesSql.TicketDetailService>();

                    services.AddTransient<RepositoryInterfacesSql.ITicketDetailRepository, RepositoryRepositoriesPostgreSql.TicketDetailRepository>();
                    services.AddTransient<RepositoryInterfacesSql.ITicketRepository, RepositoryRepositoriesPostgreSql.TicketRepository>();
                    break;
                default:
                    break;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSignalR((routes) =>
            {
                routes.MapHub<TicketHub>("/hubs/tickets");
            });

        }
    }
}