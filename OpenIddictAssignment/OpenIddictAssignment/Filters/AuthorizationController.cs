using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddictAssignment.Services;
using OpenIddictAssignment.Services.Entity;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictAssignment.Filters
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManagerFlow _userManagerFlow;
        private readonly IConfiguration _config;
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictScopeManager _scopeManager;

        /// <summary>
        /// Initialize Controller
        /// </summary>
        /// <param name="userManagerFlow"></param>
        /// <param name="config"></param>
        public AuthorizationController(UserManagerFlow userManagerFlow, IConfiguration config, IOpenIddictApplicationManager applicationManager, IOpenIddictScopeManager scopeManager)
        {
            _userManagerFlow = userManagerFlow;
            _config = config;
            _applicationManager = applicationManager;
            _scopeManager = scopeManager;
        }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        [Route("~/connect/token")]
        public async Task<IActionResult> GetToken([FromForm] UserRequestVM userRequest)
        {
            //Requesting Openiddict server to connect 
            var request = HttpContext.GetOpenIddictServerRequest() ??
             throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsPasswordGrantType())
            {
                var appId = request.GetParameter("appid").ToString() ?? string.Empty;

                //sending user credentials to check if the credentails are correct
                var userAuthenticate = _userManagerFlow.Authenticate(new LoginRequestVM()
                {
                    Password = userRequest.password ?? string.Empty,
                    Username = userRequest.username ?? string.Empty,
                });
                if (!userAuthenticate.IsAuthenticated)
                    return Forbid(authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = "Error",
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User Can't be authenticated"
                        }));

                //When ModelState is valid, get authentication payload
                //and add claims for each value i.e the name of the user.
                //So, that claims identity is constructed
                var claims = new List<Claim>();
                foreach (var claim in userAuthenticate.UserClaims)
                {
                    foreach (var item in claim.Value)
                    {
                        claims.Add(new Claim(claim.Key, item));
                    }
                }

                // Creating claims identity using
                // claims i.e name of user/password,
                // authentication Type,
                // Name Type,
                // Role Type.
                // This is basically a string.
                // Note: the client credentials are automatically validated by OpenIddict:
                // if client_id or client_secret are invalid, this action won't be invoked.
                var identity = new ClaimsIdentity(claims,
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

                // Use the client_id as the subject identifier.
                // Subject (sub) is a required field, use the client id as the subject identifier here.
                identity.SetClaim(Claims.Subject,
                    userAuthenticate.UserId.ToString());

                //This method returns the first value in the sequence of claims, which is userID.
                //userID is always the first claim added to the claims list
                //which is retrieved from default set first value from of userAuthenticate
                var userClaim = userAuthenticate?.UserClaims;
                var nameClaim = userClaim?.FirstOrDefault(x => x.Key.Contains("name")).Value?.FirstOrDefault();
                var userdataClaim = userClaim?.FirstOrDefault(x => x.Key.Contains("userdata")).Value?.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(nameClaim))
                {
                    identity.SetClaim("ClaimIds.UserId", nameClaim);
                }

                if (!string.IsNullOrWhiteSpace(userdataClaim))
                {
                    identity.SetClaim("ClaimIds.UserName", userdataClaim);
                }

                if (!string.IsNullOrWhiteSpace(appId))
                {
                    identity.SetClaim("ClaimIds.ClientAppId", appId);
                }

               
                var principal = new ClaimsPrincipal(identity);

                //OfflineAccess is used to get refresh tokens
                principal.SetScopes(Scopes.OfflineAccess);
 

                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            else if (request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the refresh token.
                var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal ?? new ClaimsPrincipal();

  

                // Ask OpenIddict to generate a new token and return an OAuth2 token response.
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
        
            throw new InvalidOperationException("The specified grant type is not supported.");
        }
    }
}
