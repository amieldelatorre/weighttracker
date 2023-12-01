using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WeightTracker.Authentication;
using WeightTracker.Data;
using WeightTracker.Models.User;
using WeightTracker.Models.Weight;

namespace WeightTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeightController : Controller
    {
        private readonly ILogger<WeightController> _logger;
        private readonly IWeightRepo _weightRepo;
        private readonly IUserRepo _userRepo;
        private readonly IAuthService _authService;

        public WeightController(ILogger<WeightController> logger, IWeightRepo weightRepo, IUserRepo userRepo, IAuthService authService)
        {
            _logger = logger;
            _weightRepo = weightRepo;
            _userRepo = userRepo;
            _authService = authService;
        }

        [Authorize]
        [HttpPost]
        [Produces("application/json")]
        async public Task<ActionResult> CreateWeight(WeightCreate weightCreateData)
        {
            try
            {
                _logger.LogDebug("Retrieving user using Claims Identity");
                User? user = await _userRepo.GetByEmail(_authService.GetEmailFromClaims());
                // In reality, if you've made it this far the user should exist
                if (user == null) 
                    return NotFound();


                _logger.LogInformation("Creating a new weight");
                bool dataIsValid = await weightCreateData.IsValid(_weightRepo, user.Id);

                if (!dataIsValid)
                {
                    _logger.LogDebug("Create new weight has invalid data");
                    return BadRequest(
                        new
                        {
                            errors = weightCreateData.GetErrors(),
                            status = HttpStatusCode.BadRequest,
                            title = "One or more validation errors occurred."
                        }
                    );
                }

                Weight newWeight = weightCreateData.CreateWeight(user.Id);
                bool add_success = await _weightRepo.Add(newWeight);

                if (!add_success) 
                {
                    _logger.LogError("Failed to create user, database insert failure");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Server is currently unable to handle your request");
                }

                WeightOutput weightOutput = new(newWeight);
                _logger.LogInformation("Successfully created a new weight");

                return CreatedAtAction(null, null, weightOutput);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create weight: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Server is currently unable to handle your request");
            }
        }
    }
}
