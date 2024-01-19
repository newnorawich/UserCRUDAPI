using Infranstructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace UserCRUDAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private UserDbContext _context;

        public UserController(UserDbContext context)
        {
            _context = context;
        }

        [HttpGet("getUser"), Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(_context.Users);
        }

        [HttpGet("getUserByName"), Authorize]
        public async Task<IActionResult> GetByName([FromQuery] string? name = "")
        {
            var user = _context.Users.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            return Ok(user);
        }

        [HttpPost("createUser"), Authorize]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (_context.Users.Any(x => x.Id == user.Id) )
            {
                return Conflict("Id already existed.");
            }   
            else if (user.Id == 0)
            {
                return BadRequest("Id must not be 0");
            }
            else if (user.Name.IsNullOrEmpty())
            {
                return BadRequest("Name must not be null or empty");
            }
            user.Name = user?.Name?.Trim();
            user.Email = user?.Email?.Trim();
            if (!Utils.emailVerification(user.Email))
            {            
                return BadRequest("Invalid Email");
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Created("Create an User in user.db", user);
        }

        [HttpPut("updateUser"), Authorize]
        public async Task<IActionResult> Update([FromQuery] int id, [FromBody] User updateInfo)
        {
            var user = _context.Users.Where(x => x.Id == (id==0 ? updateInfo.Id: id)).FirstOrDefault();
            if (user is null)
            {
                return BadRequest("Cannot find the user id " + id);
            }
            if (!Utils.emailVerification(updateInfo?.Email?.Trim()))
            {
                return BadRequest("Invalid Email");
            }
            user.Name = updateInfo.Name.IsNullOrEmpty() ? user.Name : updateInfo.Name.Trim();
            user.Email = updateInfo?.Email?.Trim() ?? user.Email;
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("deleteUser"), Authorize]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var user = _context.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user is null)
            {
                return BadRequest("Cannot find the user id " + id);
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("Remove Successfully");
        }  
    }

    public static class Utils
    {
        public static bool emailVerification(string email)
        {
            var emailValidator = new EmailAddressAttribute();
            Console.WriteLine(emailValidator.IsValid(email));
            return emailValidator.IsValid(email);
        }
    }
}
