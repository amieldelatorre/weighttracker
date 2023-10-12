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
            _logger.Log(LogLevel.Information, "User is attempting to login");
            if (await _userRepo.IsValidLogin(userLogin.Email, userLogin.Password))
            {
                _logger.Log(LogLevel.Debug, "Successful login attempt");
                return Ok();
            }

            _logger.Log(LogLevel.Debug, "Unsuccessful login attempt");
            return Unauthorized();
        }
    }
}
