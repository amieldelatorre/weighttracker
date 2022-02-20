using Microsoft.AspNetCore.Mvc;
using WeightTracker.Models;
using WeightTracker.Dtos;
using WeightTracker.Data;
using WeightTracker.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WeightTracker.Controllers
{
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger; 
        private readonly IWeightTrackerAPIRepo _repository;

        public AuthenticationController(
            IConfiguration configuration,
            ILogger<AuthenticationController> logger,
            IWeightTrackerAPIRepo repository)
        {
            _configuration = configuration;
            _logger = logger;
            _repository = repository;   
        }

        [HttpPost]
        [Route("api/[controller]/login")]
        public async Task<IActionResult> Login(LoginModel loginDetails)
        {
            if (_repository.CheckUserPassword(loginDetails.Username, loginDetails.Password))
            {
                var authclaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginDetails.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, loginDetails.Username)
                };

                var token = GetToken(authclaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("api/[controller]/register")]
        public async Task<IActionResult> Register(UserInDTO user)
        {
            var userExists = _repository.GetUserByUsername(user.UserName);
            if (userExists != null)
                return BadRequest("Username cannot be used.");

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
            return Ok(newUser); 
        }   

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SEcret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
