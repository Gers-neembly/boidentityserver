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
        private readonly AuthTokenInfo _authTokenInfo;
        #endregion

        #region Constructor
        public UACController(
            IDataAccess dataAccess,
            AuthTokenInfo authTokenInfo
            )
        {
            _dataAccess = dataAccess;
            _authTokenInfo = authTokenInfo;
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

        [BOAccessFilter(GlobalConstants.Modules.UserManagement, GlobalConstants.AccessPermission.AllowAccessToUAC)]
        [HttpGet("users/{operatorId}")]
        public async Task<IActionResult> GetBOUsers(int operatorId, int pageIndex = 1, int pageSize = 20)
        {
            var users = await Task.Run(() => _dataAccess.GetUsers(operatorId, pageIndex, pageSize));
            if (users == null)
                return NotFound(GlobalConstants.ErrUsernameAccountNotRegistered);
            return Ok(users);
        }

        #endregion
    }
}