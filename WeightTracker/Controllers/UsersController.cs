using Microsoft.AspNetCore.Mvc;
using WeightTracker.Data;
using WeightTracker.Models;
using WeightTracker.Dtos;

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

        [HttpGet("api/[controller]")]
        public IEnumerable<UserDTO> GetUsers()
        {
            IEnumerable<User> users = _repository.GetAllUsers();
            IEnumerable<UserDTO> usersOut = users.Select(user => new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                DateJoined = user.DateJoined,
                WeekStart = user.WeekStart,
                PreferredUnits = user.PreferredUnits,
                DateModified = user.DateModified
            });
            return usersOut;
        }

        [HttpGet("api/[controller]/{userId}")]
        public IActionResult GetUserById(int userId)
        {
            User? user = _repository.GetUser(userId);
            if (user == null) return NotFound();

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

            _repository.AddUser(newUser);
            return CreatedAtAction(nameof(GetUserById), new { userId = newUser.Id }, newUser);
        }

        [HttpDelete("api/[controller]/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? user = _repository.GetUser(userId);
            if (user == null) return NotFound();

            _repository.DeleteUser(user);
            return NoContent();
        }

        [HttpPut("api/[controller]/{userId}")]
        public IActionResult UpdateUser(int userId, UserInDTO userUpdate)
        {
            User? user = _repository.GetUser(userId);
            if (user == null) return NotFound();

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
