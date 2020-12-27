using AspNet.Security.OpenIdConnect.Primitives;
using DemoApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace DemoApi.Controllers
{
    [Route("/[controller]")]
    //[ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly RoleManager<UserRoleEntity> _roleManager;

        public TokenController(IOptions<IdentityOptions> identityOptions, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, RoleManager<UserRoleEntity> roleManager)
        {
            _identityOptions = identityOptions;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost(Name = nameof(TokenExchange))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> TokenExchange(OpenIdConnectRequest request)
        {
            string grantType = string.Empty;
            
            // I am not sure why I had to do that
            if (Request.Form.TryGetValue("grant_type", out var grantTypeForm))
            {
                try
                {
                    grantType = grantTypeForm.ToString();
                }
                catch (Exception e)
                {
                }
            }
            // Show the user information.
            request.GrantType = grantType;

            if (!request.IsPasswordGrantType())
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported."
                });
            }

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username or password is invalid."
                });
            }

            if (!await _signInManager.CanSignInAsync(user))
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The specified user is not allowed to sign in."
                });
            }

            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username or password is invalid."
                });
            }

            // Ensure the password is valid
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                if (_userManager.SupportsUserLockout)
                {
                    await _userManager.AccessFailedAsync(user);
                }

                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username or password is invalid."
                });
            }

            // Reset lockout count
            if (_userManager.SupportsUserLockout)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            // Look up user's roles (if any)
            var roles = new string[0];
            if (_userManager.SupportsUserRole)
            {
                roles = (await _userManager.GetRolesAsync(user)).ToArray();
            }

            // Create a new authentication ticket w/ user identity
            var principal = await CreateTicketAsync(request, user, roles);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private async Task<ClaimsPrincipal> CreateTicketAsync(OpenIdConnectRequest request, UserEntity user, string[] roles)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            AddRolesToPrincipal(principal, roles);

            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.

            // Set the list of scopes granted to the client application.
            principal.SetScopes(new[]
            {
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles
                }.Intersect(request.GetScopes()));

            //var ticket = new AuthenticationTicket(principal,
                //new AuthenticationProperties(),
                //OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            //ticket.SetScopes(OpenIdConnectConstants.Scopes.Roles);

            // Explicitly specify which claims should be included in the access token
            //foreach (var claim in ticket.Principal.Claims)
            foreach (var claim in principal.Claims)
            {
                // Never include the security stamp (it's a secret value)
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType) continue;

                // TODO: If there are any other private/secret claims on the user that should
                // not be exposed publicly, handle them here!
                // The token is encoded but not encrypted, so it is effectively plaintext.

                claim.SetDestinations(GetDestinations(claim, principal));
            }

            return principal;
        }

        private static void AddRolesToPrincipal(ClaimsPrincipal principal, string[] roles)
        {
            var identity = principal.Identity as ClaimsIdentity;

            var alreadyHasRolesClaim = identity.Claims.Any(c => c.Type == "role");
            if (!alreadyHasRolesClaim && roles.Any())
            {
                identity.AddClaims(roles.Select(r => new Claim("role", r)));
            }

            var newPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);
        }

        private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            switch (claim.Type)
            {
                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}