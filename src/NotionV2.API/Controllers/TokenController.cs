using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NotionV2.API.DTOs;
using NotionV2.API.Models;

namespace NotionV2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public TokenController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Token([FromBody] UserDTO dto)
        {
            var identity = await GetIdentity(dto.Login, dto.Password);
            if (identity == null)
            {
                return BadRequest(new {error = "Invalid login or password."});
            }

            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                issuer: _configuration["AuthOptions:Issuer"],
                audience: _configuration["AuthOptions:Audience"],
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(int.Parse(_configuration["AuthOptions:Lifetime"]))),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["AuthOptions:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                token = encodedJwt,
                login = identity.Name
            };

            return new JsonResult(response);
        }

        private async Task<ClaimsIdentity> GetIdentity(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
            if (user != null && BCrypt.Net.BCrypt.Verify(password + _configuration["Pepper"], user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login)
                };
                
                var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}