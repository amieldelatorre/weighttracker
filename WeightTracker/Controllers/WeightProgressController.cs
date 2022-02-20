using Microsoft.AspNetCore.Mvc;
using WeightTracker.Data;
using WeightTracker.Models;
using WeightTracker.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WeightTracker.Controllers
{
    [ApiController]
    public class WeightProgressController : Controller
    {
        private readonly ILogger<WeightProgressController> _logger;
        private readonly IWeightTrackerAPIRepo _repository;

        public WeightProgressController(ILogger<WeightProgressController> logger, IWeightTrackerAPIRepo repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Authorize]
        [HttpGet("api/[controller]")]
        public IEnumerable<WeightDTO> GetWeights()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            IEnumerable<WeightProgress> weights = _repository.GetWeightProgressByUserId(user.Id);
            IEnumerable<WeightDTO> weightsOut = weights.Select(weight => new WeightDTO
            {
                Id = weight.Id,
                Weight = weight.Weight,
                Date = weight.Date,
                DateCreated = weight.DateCreated,
                DateModified = weight.DateModified
            });
            return weightsOut;
        }

        [Authorize]
        [HttpGet("api/[controller]/{weightId}")]
        public IActionResult GetWeightById(int weightId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            WeightProgress? weight = _repository.GetWeight(weightId);
            if (weight == null || weight.UserId != user.Id) return NotFound();

            WeightDTO weightOut = new WeightDTO
            {
                Id = weight.Id,
                Weight = weight.Weight,
                Date = weight.Date,
                DateCreated = weight.DateCreated,
                DateModified = weight.DateModified
            };

            return Ok(weightOut);
        }

        [Authorize]
        [HttpPost("api/[controller]")]
        public ActionResult<WeightDTO> PostWeights(WeightInDTO weight)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);
            
            WeightProgress weightProgress = new WeightProgress
            {
                UserId = user.Id,
                Weight = weight.Weight,
                Date = weight.Date == DateTime.MinValue ? DateTime.Now : weight.Date,   
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };

            _repository.AddWeightProgress(weightProgress);
            return CreatedAtAction(nameof(GetWeightById), new { weightId = weightProgress.Id }, weightProgress);
        }

        [Authorize]
        [HttpDelete("api/[controller]/{weightId}")]
        public IActionResult DeleteWeight(int weightId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            WeightProgress? weight = _repository.GetWeight(weightId);
            if (weight == null || weight.UserId != user.Id) return NotFound();

            _repository.DeleteWeightProgress(weight);
            return NoContent();
        }

        [Authorize]
        [HttpPut("api/[controller]/{weightId}")]
        public IActionResult UpdateWeightProgress(int weightId, WeightInDTO weightUpdate)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            WeightProgress? weight = _repository.GetWeight(weightId);
            if (weight == null || weight.UserId != user.Id) return NotFound();

            weight.Weight = weightUpdate.Weight;
            weight.DateModified = DateTime.Now;
            _repository.UpdateWeightProgress(weight);

            return Ok(weight);
        }

        [HttpGet("api/Users/{userId}/WeightProgress")]
        public IActionResult GetWeightsByUserId(int userId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            if (userId < 0 || userId != user.Id ) return BadRequest();

            IEnumerable<WeightProgress> weights = _repository.GetWeightProgressByUserId(userId);
            IEnumerable<WeightDTO> weightsOut = weights.Select(weight => new WeightDTO
            {
                Id = weight.Id,
                Weight = weight.Weight,
                DateCreated = weight.DateCreated,
                DateModified = weight.DateModified
            });
            
            return Ok(weightsOut);
        }

        [Authorize]
        [HttpPost("api/Users/{userId}/WeightProgress")]
        public IActionResult AddWeightByUserId(int userId, WeightInDTO weight)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string username = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            User user = _repository.GetUserByUsername(username);

            if (userId != user.Id) return BadRequest(); 

            WeightProgress weightProgress = new WeightProgress
            {
                UserId = userId,
                Weight = weight.Weight,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };

            _repository.AddWeightProgress(weightProgress);
            return CreatedAtAction(nameof(GetWeightById), new { weightId = weightProgress.Id }, weightProgress);
        }
    }
}
