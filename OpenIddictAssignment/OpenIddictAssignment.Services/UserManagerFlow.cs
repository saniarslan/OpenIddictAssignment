using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenIddictAssignment.Services.Entity;

namespace OpenIddictAssignment.Services
{
    public class UserManagerFlow(IConfiguration config,
       ILogger<UserManagerFlow> logger, IMapper mapper)
    {

        #region Properties

        private readonly ILogger<UserManagerFlow> _logger = logger;

        #endregion

        #region Const
        private readonly IConfiguration _config = config;
        private readonly IMapper _mapper = mapper;
        private protected IServiceProvider _serviceProvider;

        #endregion

        public LoginResponseVM Authenticate(LoginRequestVM loginRequest)
        {
            var response = new LoginResponseVM
            {
                IsAuthenticated = false,
                Active = false,
                IsLocked = false,
                UserId = Guid.Empty
            };
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Calling '{nameof(Authenticate)}' method in '{nameof(UserManagerFlow)}'.");

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Params of '{nameof(Authenticate)}' method in '{nameof(UserManagerFlow)}': loginRequest {loginRequest}.");



            //Validate User to check if it's Exist in DB
            var user = UserExists(loginRequest.Username);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"User Response: {user}");

            //Check if this a genuine(Authenticated) user
            if (IsUserAuthenticated($"{user?.Username}", loginRequest.Password))
            {
               
                return response;
            }

            //If user is not authenticated
            response.IsAuthenticated = false;

            return userResponse;
        }
        private bool UserExists(string username) => return true();
        private bool IsUserAuthenticated(string username, string password, string authGatewayId, string srcTypeId)
        {
            return true;
        }
    }

}
