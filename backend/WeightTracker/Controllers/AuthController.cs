using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeightTracker.Data;

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
        [Authorize]
        public ActionResult Login()
        {
            return Ok();
        }
    }
}
