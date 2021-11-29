using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotionV2.API.DTOs;
using NotionV2.API.Models;

namespace NotionV2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get([FromRoute] int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] UserDTO dto)
        {
            if (await _context.Users.AnyAsync(x => x.Login == dto.Login))
            {
                return BadRequest(new {error = "User with this login already exists."});
            }
            
            var user = new User
            {
                Login = dto.Login,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password + _configuration["Pepper"])
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new {id = user.Id}, user);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] UserDTO dto)
        {
            if (await _context.Users.AnyAsync(x => x.Login == dto.Login) && dto.Login != User.Identity?.Name)
            {
                return BadRequest(new {error = "User with this login already exists."});
            }
            
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
            user.Login = dto.Login;
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password + _configuration["Pepper"]);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}