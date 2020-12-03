using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using Neembly.BOIDServer.WebAPI.Services;

namespace Neembly.BOIDServer.WebAPI
{
    public static class DependencyInjection
    {
        /// <summary>
        /// DI for services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection Add(this IServiceCollection services)
        {

            //identiy claims
            services.AddTransient<IProfileService, IdentityClaimsProfileService>();

            return services;
        }
    }
}
