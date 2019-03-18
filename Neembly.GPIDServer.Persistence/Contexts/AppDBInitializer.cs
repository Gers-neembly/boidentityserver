using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Helpers;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.Persistence.Seeders;
using System;

namespace Neembly.BOIDServer.Persistence.Contexts
{
    public class AppDBInitializer
    {
        public static void Seeder(IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            var context = serviceProvider.GetRequiredService<AppDBContext>();
            var dataAccess = serviceProvider.GetRequiredService<IDataAccess>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var environment = env.EnvironmentName;

            context.Database.EnsureCreated();
            BOUsers.SeedUserData(dataAccess, userManager, roleManager, environment);
        }

    }
}
