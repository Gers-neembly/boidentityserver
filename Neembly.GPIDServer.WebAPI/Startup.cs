using FluentValidation.AspNetCore;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neembly.BOIDServer.Persistence.Contexts;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Helpers;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.SharedServices.Helpers;
using Neembly.BOIDServer.SharedServices.Interfaces;
using Neembly.BOIDServer.WebAPI.Filters;
using Neembly.BOIDServer.WebAPI.Models.Configs;
using Neembly.BOIDServer.WebAPI.Services;

namespace Neembly.BOIDServer.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add DBContext services
            services.AddDbContext<AppDBContext>(options =>
                                               options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            //// Add Identity services
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

            var authClientConfig = new AuthClientConfiguration();
            Configuration.Bind("AuthClientConfiguration", authClientConfig);
            services.AddSingleton(authClientConfig);

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryPersistedGrants()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources(authClientConfig.AuthClientResourcesList))
                    .AddInMemoryClients(Config.GetClients(authClientConfig.AuthClientInfoList))
                    .AddAspNetIdentity<AppUser>();

            services.Configure<IdentityOptions>(o => {
                o.SignIn.RequireConfirmedEmail = false;
            });

            // dependency injections
            services.AddScoped<IDataAccess, DataAccess>();
            services.AddScoped<IEmailDispatcher, EmailDispatcher>();
            services.AddTransient<IProfileService, IdentityClaimsProfileService>();


            services.AddCors();
            services
                .AddMvc(options => options.Filters.Add(typeof(CustomExceptionFilterAttribute)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(typeof(Exceptions.ValidationException).Assembly));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //// global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            AppDBInitializer.Seeder(app.ApplicationServices, env);

            app.UseIdentityServer();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
