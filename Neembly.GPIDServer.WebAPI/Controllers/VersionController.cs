using Microsoft.AspNetCore.Mvc;
using Neembly.BOIDServer.WebAPI.Models;
using System;
using System.Reflection;

namespace Neembly.BOIDServer.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var versionTag = new VersionInfo
            {
                ProviderName = "Neembly BackOffice Identity Host Service",
                Version = "Version 1.0.0",
                BuildNo = $"Build {Assembly.GetEntryAssembly().GetName().Version}",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Cluster = Environment.GetEnvironmentVariable("ASPNETCORE_CLUSTER")
            };
            return new JsonResult(versionTag);
        }
    }
}