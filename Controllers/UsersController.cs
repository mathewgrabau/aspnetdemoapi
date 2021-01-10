using DemoApi.Models;
using DemoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly PagingOptions _defaultPagingOptions;
        private readonly IAuthorizationService _authorizationService;

        public UsersController(
            IUserService userService,
            IOptions<PagingOptions> defaultPagingOptions,
            IAuthorizationService authorizationService)
        {
            _userService = userService;
            _defaultPagingOptions = defaultPagingOptions.Value;
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = nameof(GetVisibleUsers))]
        public async Task<ActionResult<PagedCollection<User>>> GetVisibleUsers(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<User, UserEntity> sortOptions,
            [FromQuery] SearchOptions<User, UserEntity> searchOptions)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            // TODO: Authorization check. Is the user an admin?
            var users = new PagedResults<User>();
            if (User.Identity.IsAuthenticated)
            {
                var canSeeAllUsers = await _authorizationService.AuthorizeAsync(User, "ViewAllUsersPolicy");
                if (canSeeAllUsers.Succeeded)
                {
                    // Admin, view everyone
                    users = await _userService.GetUsersAsync(pagingOptions, sortOptions, searchOptions);
                }
                else
                {
                    // Only return the one user (all that we can see)
                    var myself = await _userService.GetUserAsync(User);
                    users.Items = new[]
                    {
                        myself
                    };
                    users.TotalSize = 1;
                }
            }

            var collection = PagedCollection<User>.Create(Link.To(nameof(GetVisibleUsers)), users.Items.ToArray(), users.TotalSize, pagingOptions);

            return collection;
        }

        [Authorize]
        [ProducesResponseType(401)]
        [HttpGet("{userId}", Name = nameof(GetUserById))]
        public Task<IActionResult> GetUserById(Guid userId)
        {
            // TODO is userId the current user's ID?
            // If so, return myself.
            // If not, only Admin roles should be able to view arbitrary users.
            throw new NotImplementedException();
        }

        // POST /users
        [HttpPost(Name = nameof(RegisterUser))]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterForm registerForm)
        {
            var (succeeded, message) = await _userService.CreateUserAsync(registerForm);
            if (succeeded)
            {
                return Created(Url.Link(nameof(UserInfoController.UserInfo), null), null);
            }

            return BadRequest(new ApiError
            {
                Message = "Registration failed",
                Detail = message
            });
        }
    }
}