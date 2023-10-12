namespace WeightTracker.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthService(IHttpContextAccessor contextAccessor) 
        { 
            _contextAccessor = contextAccessor;
        }

        public string GetEmailFromClaims()
        {
            var result = string.Empty;
            if (_contextAccessor.HttpContext is not null)
            {
                result = _contextAccessor.HttpContext.User?.Claims.First().Value;
            }
            return result;
        }
    }
}
