using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WeightTracker.Data;

namespace WeightTracker.Authentication
{
    public class BasicAuthenticationHandler(IUserRepo userRepo, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        private readonly IUserRepo _userRepo = userRepo;

        async protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value))
            {
                Response.Headers.Append("WWW-Authenticate", "Basic");
                return AuthenticateResult.Fail("Authorization header not found");
            }

#pragma warning disable CS8604 // Possible null reference argument.
            AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(value);
#pragma warning restore CS8604 // Possible null reference argument.
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
                ClaimsPrincipal principal = new(identity);
                AuthenticationTicket ticket = new(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            else
                return AuthenticateResult.Fail("Incorrect login details");
        }
    }
}
