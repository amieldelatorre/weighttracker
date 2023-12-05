namespace WeightTracker.Authentication
{
    public class AuthService(IHttpContextAccessor contextAccessor) : IAuthService
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public string GetEmailFromClaims()
        {
            var result = string.Empty;
            if (_contextAccessor.HttpContext is not null)
            {
                result = _contextAccessor.HttpContext.User?.Claims.First().Value;
            }
#pragma warning disable CS8603 // Possible null reference return.
            return result;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
