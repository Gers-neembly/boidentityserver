using Microsoft.Extensions.DependencyInjection;
using Neembly.BOIDServer.SharedServices.Helpers;
using Neembly.BOIDServer.SharedServices.Interfaces;

namespace Neembly.BOIDServer.SharedServices
{
    public static class DependencyInjection
    {
        /// <summary>
        /// DI for shared services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection Add(this IServiceCollection services)
        {
            //email
            services.AddScoped<IEmailDispatcher, EmailDispatcher>();

            return services;
        }
    }
}
