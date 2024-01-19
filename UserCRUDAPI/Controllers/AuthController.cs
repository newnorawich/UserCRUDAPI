using Infranstructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserCRUDAPI.Auth;

namespace UserCRUDAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        //public static UserAuth user = new UserAuth();
        private readonly IConfiguration _configuration;
        private readonly UserDbContext _context;

        public AuthController(IConfiguration configuration, UserDbContext context) 
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserAuthRequest request)
        {
            if (_context.UserAuth.Any(x => x.Username == request.Username))
            {
                return Conflict("Username has been taken.");
            }
            var user = new UserAuth()
            {
                Username = request.Username,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            await _context.UserAuth.AddAsync(user);
            await _context.SaveChangesAsync();
            
            return Ok(user.Username); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] UserAuthRequest request)
        {
            var user = _context.UserAuth.Where(x => x.Username == request.Username).FirstOrDefault();
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.HashPassword))
            {
                return BadRequest("Username or Password is wrong.");
            }
            var jwt = CreateToken(user);
            return Ok(jwt);
        }

        private string CreateToken (UserAuth user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var securityKey = _configuration.GetSection("AppSettings:Token").Value;
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            var payload = new JwtSecurityToken(null, null, claims, null, DateTime.Today.AddDays(1), credentials);

            return new JwtSecurityTokenHandler().WriteToken(payload);
        }
    }
}
