using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neembly.BOIDServer.SharedServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.WebAPI.Filters
{
    public class NeemblyAuthorizeAttribute : TypeFilterAttribute
    {

        public NeemblyAuthorizeAttribute()
            : base(typeof(NeemblyAuthorizeFilter))
        {

        }

    }

    public class NeemblyAuthorizeFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string auth = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(auth))
            {
                string accessToken = auth.Substring(7);
                var serviceToken = context.HttpContext.RequestServices.GetService<ITokenProviderService>();
                if (!await serviceToken.ValidateToken(accessToken))
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
