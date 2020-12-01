using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Neembly.BOIDServer.SharedClasses
{
    public static class DependencyInjection
    {
        /// <summary>
        /// DI for shared classes - config
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection Add(this IServiceCollection services, IConfiguration configuration)
        {
            // authentication token info config
            var authTokenConfig = new AuthTokenInfo();
            configuration.Bind("AuthTokenInfo", authTokenConfig);
            services.AddSingleton(authTokenConfig);

            return services;
        }
    }
}
