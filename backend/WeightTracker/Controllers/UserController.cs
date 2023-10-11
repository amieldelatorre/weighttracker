using Microsoft.AspNetCore.Mvc;
using System.Net;
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

        public UserController(ILogger<UserController> logger, IUserRepo userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        [HttpPost]
        [Produces("application/json")]
        async public Task<ActionResult> CreateUser(UserCreate userCreateData)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Creating new user.");

                if (!userCreateData.IsValid(_userRepo))
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
