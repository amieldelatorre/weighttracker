using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Numerics;
using WeightTracker.Authentication;
using WeightTracker.Controllers.QueryParameters;
using WeightTracker.Data;
using WeightTracker.Models;
using WeightTracker.Models.User;
using WeightTracker.Models.Weight;

namespace WeightTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeightController(ILogger<WeightController> logger, IWeightRepo weightRepo, IUserRepo userRepo, IAuthService authService) : Controller
    {
        private readonly ILogger<WeightController> _logger = logger;
        private readonly IWeightRepo _weightRepo = weightRepo;
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IAuthService _authService = authService;

        [Authorize]
        [HttpGet("{weightId:int}")]
        [Produces("application/json")]
        async public Task<ActionResult> GetWeightById(int weightId)
        {
            try
            {
                _logger.LogDebug("Retrieving user using Claims Identity");
                User? user = await _userRepo.GetByEmail(_authService.GetEmailFromClaims());
                // In reality, if you've made it this far the user should exist
                if (user == null)
                    return NotFound();

                _logger.LogInformation("Retrieving weight");
                Weight? weight = await _weightRepo.GetById(user.Id, weightId);

                if (weight == null) return NotFound();

                WeightOutput weightOutput = new(weight);
                return Ok(weightOutput);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get weight with Id {weightId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Server is currently unable to handle your request");
            }
        }

        [Authorize]
        [HttpGet]
        [Produces("application/json")]
        async public Task<ActionResult<PaginatedResult<Weight>>> GetWeights([FromQuery] GetWeightsQueryParameters queryParameters)
        {
            try
            {
                _logger.LogDebug("Retrieving user using Claims Identity");
                User? user = await _userRepo.GetByEmail(_authService.GetEmailFromClaims());
                // In reality, if you've made it this far the user should exist
                if (user == null)
                    return NotFound();

                _logger.LogInformation("Retrieving weightsQuery");
                FilteredResult<Weight> filteredResults = await GetFilteredWeights(user.Id, queryParameters);

                string? nextPath = null;
                if (queryParameters.Limit + queryParameters.Offset < filteredResults.Total)
                {
                    nextPath = $"{Request.Path.Value}?limit={queryParameters.Limit}&offset={queryParameters.Limit + queryParameters.Offset}";
                    if (queryParameters.DateFrom != null)
                        nextPath += $"&datefrom={queryParameters.DateFrom?.ToString("yyyy/MM/dd")}";
                    if (queryParameters.DateTo != null)
                        nextPath += $"&dateto={queryParameters.DateTo?.ToString("yyyy/MM/dd")}";
                }

                PaginatedResult<Weight> result = new()
                {
                    Results = filteredResults.Results,
                    Total = filteredResults.Total,
                    Limit = queryParameters.Limit,
                    Offset = queryParameters.Offset,
                    Next = nextPath
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get weightsQuery: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Server is currently unable to handle your request");
            }
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

        async private Task<FilteredResult<Weight>> GetFilteredWeights(int userId, GetWeightsQueryParameters queryParameters)
        {
            IQueryable<Weight> weightsQuery = _weightRepo.GetAllByUserId(userId);

            if (queryParameters.DateFrom != null)
                weightsQuery = weightsQuery.Where(weight => weight.Date >= queryParameters.DateFrom);
            if (queryParameters.DateTo != null)
                weightsQuery = weightsQuery.Where(weight => weight.Date <= queryParameters.DateTo);

            weightsQuery = weightsQuery.OrderByDescending(weight => weight.Date);
            List<Weight> weights = await weightsQuery.ToListAsync();

            int total = weights.Count;
            weights = weights.Skip(queryParameters.Offset).Take(queryParameters.Limit).ToList();
            FilteredResult<Weight> filteredResult = new()
            {
                Total = total,
                Results = weights
            };
            return filteredResult;
        }
    }
}
