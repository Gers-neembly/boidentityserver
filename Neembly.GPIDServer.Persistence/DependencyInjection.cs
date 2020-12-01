using Microsoft.Extensions.DependencyInjection;
using Neembly.BOIDServer.Persistence.Helpers;
using Neembly.BOIDServer.Persistence.Interfaces;

namespace Neembly.BOIDServer.Persistence
{
    public static class DependencyInjection
    {
        /// <summary>
        /// DI for persistence
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection Add(this IServiceCollection services) 
        {
            //data access
            services.AddScoped<IDataAccess, DataAccess>();

            return services;
        }
    }
}
