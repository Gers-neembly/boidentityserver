using Microsoft.AspNetCore.Mvc;
using Neembly.BOIDServer.SharedClasses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Neembly.BOIDServer.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UACController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            using (StreamReader r = new StreamReader("uacmenulist.json"))
            {
                string json = r.ReadToEnd();
                List<UACMenuInfo> menus = JsonConvert.DeserializeObject<List<UACMenuInfo>>(json);
                return new JsonResult(menus);
            }
        }
    }
}