using System.Threading.Tasks;
using EducationApp.BusinessLogicLayer.Models.Users;
using Microsoft.AspNetCore.Mvc;
using EducationApp.BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authorization;
using EducationApp.BusinessLogicLayer.Extention.User;
using role = EducationApp.BusinessLogicLayer.Common.Consts.Constants.UserRoles;

namespace EducationApp.PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = role.Admin)]
        [HttpGet("blockUser")]
        public async Task<IActionResult> BlockUser(long id)
        {
           var result = await _userService.BlockUserAsync(id);

            return Ok(result);
        }
        [Authorize(role.Admin)]
        [HttpGet("getProfile")]
        public async Task<UserModelItem> GetProfile(long id)
        {
            var user = User;
            var profile = await _userService.GetProfileAsync(id);

            return profile;
        }

        [Authorize(Roles = role.Admin)]
        [HttpGet("remove")]
        public async Task<ActionResult> RemoveUser(long id)
        {
            var result = await _userService.RemoveUserAsync(id);

            return Ok(result);
        }

        //[Authorize(Roles = role.User)]
        [HttpPost("editProfile")]
        public async Task<ActionResult> EditProfile(UserProfileEditModel model)
        {
            var result = await _userService.EditProfileAsync(model);  
            
            return Ok(result);
        }

        //[Authorize(Roles = role.User)]
        [HttpGet("changePassword")]
        public async Task<ActionResult> ChangePassword(long id, string oldPassword,string newPassword)
        {
            var result = await _userService.ChangePassword(id, oldPassword, newPassword);

            return Ok(result);
        }

        [Authorize(Roles = role.Admin)]
        [HttpPost("getUsers")]
        public async Task<ActionResult> GetUsers(UserFilterModel filterUser)
       {
            var users = await _userService.GetUsersAsync(filterUser);

            return Ok(users);
        }


    }
}
