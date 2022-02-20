using Microsoft.AspNetCore.Mvc;
using WeightTracker.Data;
using WeightTracker.Models;
using WeightTracker.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WeightTracker.Exceptions;

namespace WeightTracker.Controllers
{
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IWeightTrackerAPIRepo _repository;

        public UsersController(ILogger<UsersController> logger, IWeightTrackerAPIRepo repository)
        {
            _logger = logger;
            _repository = repository;   
        }

        [Authorize]
        [HttpGet("api/[controller]")]
        public UserDTO GetUsers()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            UserDTO userOut = new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                DateJoined = user.DateJoined,
                WeekStart = user.WeekStart,
                PreferredUnits = user.PreferredUnits,
                DateModified = user.DateModified
            };
            return userOut;
        }

        [Authorize]
        [HttpGet("api/[controller]/{userId}")]
        public IActionResult GetUserById(int userId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            if (userId != user.Id) return NotFound();

            return Ok(user);
        }

        [HttpPost("api/[controller]")]
        public ActionResult<UserDTO> PostUser(UserInDTO user)
        {
            User newUser = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Password = user.Password,
                DateJoined = DateTime.Now,
                WeekStart = user.WeekStart,
                PreferredUnits = user.PreferredUnits,
                DateModified = DateTime.Now
            };

            try
            {
                _repository.AddUser(newUser);
            }
            catch (UsernameAlreadyExistsException ex)
            {
                return BadRequest(ex.Message);
            }
            return CreatedAtAction(nameof(GetUserById), new { userId = newUser.Id }, newUser);
        }

        [Authorize]
        [HttpDelete("api/[controller]/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            if (userId != user.Id) return NotFound();

            _repository.DeleteUser(user);
            return NoContent();
        }

        [Authorize]
        [HttpPut("api/[controller]/{userId}")]
        public IActionResult UpdateUser(int userId, UserInDTO userUpdate)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            if (userId != user.Id) return NotFound();

            user.FirstName = userUpdate.FirstName;
            user.LastName = userUpdate.LastName;
            user.UserName = userUpdate.UserName;
            user.Password = userUpdate.Password;
            user.WeekStart = userUpdate.WeekStart;
            user.PreferredUnits = userUpdate.PreferredUnits;
            user.DateModified = DateTime.Now;
            _repository.UpdateUser(user);

            return Ok(user);
        }
    }
}
