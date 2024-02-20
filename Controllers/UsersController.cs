/* using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo_List.Models;

namespace Todo_List.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TodoDBContext _dbContext;

        public UsersController(TodoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Users
        [HttpGet]

        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_dbContext.Users == null)
            {
                return NotFound();
            }
            return await _dbContext.Users.ToListAsync();
        }

        // POST: api/Users
        [HttpPost]

        public async Task<ActionResult<User>> PostUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user?.UserId }, user);
        }
    }
} */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo_List.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Todo_List.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly TodoDBContext _dbContext;

        public UsersController( IConfiguration config, TodoDBContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var usersWithTasks = await _dbContext.Users
                    .Include(u => u.Tasks) // Include tasks related to each user
                    .ToListAsync();

                if (usersWithTasks == null || usersWithTasks.Count == 0)
                {
                    return NotFound("No tasks available to this user.");
                }

                return Ok(usersWithTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody]User user)
        {

            try
            {
                if (await _dbContext.Users.AnyAsync(u => u.Email == user.Email))
                {
                    return BadRequest("Email is already taken");
                }

                string salt = BCrypt.Net.BCrypt.GenerateSalt();

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                user.PasswordHash = null;

                return CreatedAtAction(nameof(GetUsers), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserLoginModel model)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            var userId = user.UserId;
            var email = user.Email;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_config["Jwt:ValidIssuer"],
              _config["Jwt:ValidIssuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return Ok(new { Token = token,UserId = userId, Email = email  });
        }
    }
}

