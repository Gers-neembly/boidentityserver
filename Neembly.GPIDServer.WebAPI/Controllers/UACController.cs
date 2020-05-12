using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Neembly.BOIDServer.Constants;
using Neembly.BOIDServer.Persistence.Entities;
using Neembly.BOIDServer.Persistence.Interfaces;
using Neembly.BOIDServer.SharedClasses;
using Neembly.BOIDServer.SharedServices.Interfaces;
using Neembly.BOIDServer.WebAPI.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Neembly.BOIDServer.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UACController : ControllerBase
    {
        #region Member Variable
        private readonly IDataAccess _dataAccess;
        private readonly ITokenProviderService _tokenProviderService;
        #endregion

        #region Constructor
        public UACController(
            IDataAccess dataAccess,
            ITokenProviderService tokenProviderService
            )
        {
            _dataAccess = dataAccess;
            _tokenProviderService = tokenProviderService;
        }
        #endregion


        #region READ

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

        [Route("user-profile")]
        [HttpGet]
        public async Task<IActionResult> GetUserProfile(string username)
        {
                var userInfo = await Task.Run(() => _dataAccess.GetUserInfo(username));
                if (userInfo == null)
                    return NotFound(GlobalConstants.ErrUsernameAccountNotRegistered);

                return Ok(userInfo);
        }

        [HttpGet("users/{operatorId}")]
        public async Task<IActionResult> GetBOUsers(int operatorId)
        {
                var users = await Task.Run(() => _dataAccess.GetUsers(operatorId));
                if (users == null)
                    return NotFound(GlobalConstants.ErrUsernameAccountNotRegistered);

                return Ok(users);
        }

        #endregion
    }
}