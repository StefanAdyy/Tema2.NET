using Core.Services;
using DataLayer.Dtos;
using DataLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();

        private AuthService authService { get; set; }
        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            authService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Id = request.Id;
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = new Role(request.Role);

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login (UserLoginDto request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if(!authService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = authService.CreateToken(user);
            return Ok(token);
        }
    }
}
