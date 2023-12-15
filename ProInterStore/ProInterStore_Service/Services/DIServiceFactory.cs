using Microsoft.Extensions.DependencyInjection;
using ProInterStore_DataAccess.Interfaces;
using ProInterStore_DataAccess.Repositories;
using ProInterStore_Service.Interfaces;
using ProInterStore_Shared.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterStore_Service.Services
{
    public static class DIServiceFactory
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(AutoMapperConfig.Configure());

            services.AddTransient(typeof(JWTInterceptorAuthFilter));

            services.AddTransient(typeof(IStoreItemRepository), typeof(StoreItemRepository));
            services.AddTransient(typeof(IStoreItemService), typeof(StoreItemService));
            services.AddTransient(typeof(IUserRepository), typeof(UserRepository));

            services.AddTransient(typeof(IAuditInfoRepository), typeof(AuditInfoRepository));

            services.AddTransient(typeof(IUserService), typeof(UserService));
            services.AddTransient(typeof(ILoggedUsersInfoRepository), typeof(LoggedUsersInfoRepository));
            services.AddTransient(typeof(ITokenService), typeof(TokenService));
        }
    }
}
