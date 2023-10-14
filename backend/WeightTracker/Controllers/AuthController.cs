using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeightTracker.Data;
using WeightTracker.Models;

namespace WeightTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserRepo _userRepo;
        
        public AuthController(ILogger<AuthController> logger, IUserRepo userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        [HttpPost("login")]
        async public Task<ActionResult> Login(UserLogin userLogin)
        {
            _logger.LogInformation("User is attempting to login");
            if (await _userRepo.IsValidLogin(userLogin.Email, userLogin.Password))
            {
                _logger.LogDebug("Successful login attempt");
                return Ok();
            }

            _logger.LogDebug("Unsuccessful login attempt");
            return Unauthorized();
        }
    }
}
