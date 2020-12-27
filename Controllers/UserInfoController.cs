using AspNet.Security.OpenIdConnect.Primitives;
using DemoApi.Models;
using DemoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DemoApi.Controllers
{
    [Route("/[controller]")]
    [Authorize]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UserInfoController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /userinfo
        [HttpGet(Name = nameof(UserInfo))]
        [ProducesResponseType(401)]
        public async Task<ActionResult<UserInfoResponse>> UserInfo()
        {
            var user = await _userService.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "User does not exist"
                });
            }

            var userId = _userService.GetUserIdAsync(User);

            return new UserInfoResponse
            {
                Self = Link.To(nameof(UserInfo)),
                GivenName = user.FirstName,
                FamilyName = user.LastName,
                Subject = Url.Link(nameof(UsersController.GetUserById), new { userId })
            };
        }
    }
}