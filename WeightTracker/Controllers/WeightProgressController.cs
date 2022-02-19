using Microsoft.AspNetCore.Mvc;
using WeightTracker.Data;
using WeightTracker.Models;
using WeightTracker.Dtos;

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

        [HttpGet("api/[controller]")]
        public IEnumerable<WeightDTO> GetWeights()
        {
            IEnumerable<WeightProgress> weights = _repository.GetAllWeights();
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

        [HttpGet("api/[controller]/{weightId}")]
        public IActionResult GetWeightById(int weightId)
        {
            WeightProgress? weight = _repository.GetWeight(weightId);
            if (weight == null) return NotFound();

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

        [HttpPost("api/[controller]")]
        public ActionResult<WeightDTO> PostWeights(WeightInDTO weight)
        {
            User? user = _repository.GetUser(weight.UserId);
            if (user == null) return NotFound();
            
            WeightProgress weightProgress = new WeightProgress
            {
                UserId = weight.UserId,
                Weight = weight.Weight,
                Date = weight.Date == DateTime.MinValue ? DateTime.Now : weight.Date,   
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };

            _repository.AddWeightProgress(weightProgress);
            return CreatedAtAction(nameof(GetWeightById), new { weightId = weightProgress.Id }, weightProgress);
        }

        [HttpDelete("api/[controller]/{weightId}")]
        public IActionResult DeleteWeight(int weightId)
        {
            WeightProgress? weight = _repository.GetWeight(weightId);
            if (weight == null) return NotFound();

            _repository.DeleteWeightProgress(weight);
            return NoContent();
        }

        [HttpPut("api/[controller]/{weightId}")]
        public IActionResult UpdateWeightProgress(int weightId, WeightInDTO weightUpdate)
        {
            WeightProgress? weight = _repository.GetWeight(weightId);
            if (weight == null) return NotFound();

            weight.Weight = weightUpdate.Weight;
            weight.DateModified = DateTime.Now;
            _repository.UpdateWeightProgress(weight);

            return Ok(weight);
        }

        [HttpGet("api/Users/{userId}/WeightProgress")]
        public IActionResult GetWeightsByUserId(int userId)
        {
            if (userId < 0) return BadRequest();
            User? user = _repository.GetUser(userId);
            if (user == null) return NotFound();

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

        [HttpPost("api/Users/{userId}/WeightProgress")]
        public IActionResult AddWeightByUserId(int userId, WeightInDTO weight)
        {
            User? user = _repository.GetUser(userId);
            if (user == null) return NotFound();
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
