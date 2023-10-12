using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using WeightTracker.Authentication;
using WeightTracker.Data;
using WeightTracker.Models;

namespace WeightTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepo _userRepo;
        private readonly IAuthService _authService;

        public UserController(ILogger<UserController> logger, IUserRepo userRepo, IAuthService authService)
        {
            _logger = logger;
            _userRepo = userRepo;
            _authService = authService;
        }

        [Authorize]
        [HttpGet]
        [Produces("application/json")]
        async public Task<ActionResult> GetUserFromCredentials()
        {
            try
            {
                _logger.Log(LogLevel.Debug, "Retrieving user using Claims Identity");

                User? user = await _userRepo.GetByEmail(_authService.GetEmailFromClaims());
                // In reality, if you've made it this far the user should exist
                if (user == null)
                    return NotFound();
                UserOutput userOutput = new(user);
                _logger.Log(LogLevel.Information, "Retrieved user using Claims Identity");

                return Ok(userOutput);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Failed to retrieve user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Server is currently unable to handle your request");
            }
            
        }

        [HttpPost]
        [Produces("application/json")]
        async public Task<ActionResult> CreateUser(UserCreate userCreateData)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Creating new user.");
                bool dataIsValid = await userCreateData.IsValid(_userRepo);

                if (!dataIsValid)
                {
                    _logger.Log(LogLevel.Debug, "Create new user has invalid data.");
                    return BadRequest(
                        new
                        {
                            errors = userCreateData.GetErrors(),
                            status = HttpStatusCode.BadRequest,
                            title = "One or more validation errors occurred."
                        }
                    );
                }

                User newUser = userCreateData.CreateUser();
                bool add_success = await _userRepo.Add(newUser);

                if (!add_success)
                {
                    _logger.Log(LogLevel.Error, "Failed to create user, database insert failure");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Server is currently unable to handle your request");
                }

                UserOutput userOutput = new(newUser);
                _logger.Log(LogLevel.Information, "Successfully created a new user.");

                return CreatedAtAction(null, null, userOutput);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Failed to create user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Server is currently unable to handle your request");
            }
            
        }
    }
}
