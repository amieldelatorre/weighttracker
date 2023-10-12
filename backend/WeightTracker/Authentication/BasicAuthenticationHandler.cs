using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WeightTracker.Data;

namespace WeightTracker.Authentication
{
    public class BasicAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserRepo _userRepo;

        public BasicAuthenticationHandler(IUserRepo userRepo, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _userRepo = userRepo;
        }

        async protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Response.Headers.Add("WWW-Authenticate", "Basic");
                return AuthenticateResult.Fail("Authorization header not found");
            }

            AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader.Parameter == null)
                return AuthenticateResult.Fail("Authorization header found but is empty.");
            
            string[] authHeaderItems = authHeader.Parameter.Split();
            if (authHeaderItems.Length != 1)
                return AuthenticateResult.Fail("Incorrect Authorization header format. Format is Authorization: Basic {Credentials}");

            byte[] credentialBytes = Convert.FromBase64String(authHeaderItems[0]);
            string[] credentialsArray = Encoding.UTF8.GetString(credentialBytes).Split(":");
            string email = credentialsArray[0];
            string password = credentialsArray[1];

            if (await _userRepo.IsValidLogin(email, password))
            {
                var claims = new[] { new Claim("Email", email) };
                ClaimsIdentity identity = new(claims, "Basic");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                AuthenticationTicket ticket = new(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            else
                return AuthenticateResult.Fail("Incorrect login details");
        }
    }
}
