using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neembly.BOIDServer.Persistence.Contexts;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.WebAPI.Models.Configs;

namespace Neembly.BOIDServer.WebAPI
{
    public static class DependencyInjectionConfigs
    {
        /// <summary>
        /// DI for configurations
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection Add(this IServiceCollection services, IConfiguration configuration)
        {
            //application database context
            services.AddDbContext<AppDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            //identity service
            services.AddIdentity<AppUser, IdentityRole>(user =>
            {
                // configure identity options
                user.Password.RequireDigit = false;
                user.Password.RequireLowercase = false;
                user.Password.RequireUppercase = false;
                user.Password.RequireNonAlphanumeric = false;
                user.Password.RequiredLength = 2;
            })
                .AddEntityFrameworkStores<AppDBContext>()
                .AddDefaultTokenProviders();

            //authentication client config
            var authClientConfig = new AuthClientConfiguration();
            configuration.Bind("AuthClientConfiguration", authClientConfig);
            services.AddSingleton(authClientConfig);

            //add identity server
            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryPersistedGrants()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources(authClientConfig.AuthClientResourcesList))
                    .AddInMemoryClients(Config.GetClients(authClientConfig.AuthClientInfoList))
                    .AddAspNetIdentity<AppUser>();

            //config for options to configure identity system
            services.Configure<IdentityOptions>(o => {
                o.SignIn.RequireConfirmedEmail = false;
            });

            //add cross origin 
            services.AddCors();

            //mvc service
            services.AddMvc(options => 
                    options.Filters.Add(typeof(Filters.CustomExceptionFilterAttribute)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(typeof(Exceptions.ValidationException).Assembly));

            return services;
        }
    }
}
