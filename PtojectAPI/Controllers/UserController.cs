using System.Security.Cryptography;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PtojectAPI.Datas;
using PtojectAPI.Entitys;
using PtojectAPI.Models;
using PtojectAPI.Services;

namespace PtojectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtService _jwtService;

        private readonly DBContext _context;
        public UserController(DBContext context, JwtService jwtService)
        {
            this._context = context;
            this._jwtService = jwtService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.Email == register.Email))
                return BadRequest("Email already exists");

            string hashedpassword = BCrypt.Net.BCrypt.HashPassword(register.Password);

            var user = new User
            {
                Name = register.Name,
                Email = register.Email,
                PasswordHash = hashedpassword //register.Password   
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "user registered sucessfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            var user = await _context.Users.Include(u => u.RefreshTokens)
                                  .FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null)
                return BadRequest("Invalid credentials");
            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))

            return BadRequest("Invalid credentials");
            //return Ok(new { message = "user logged in successfully!" });
            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            //user.RefreshToken = refreshToken;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            refreshToken.UserId = user.UserId;
            user.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();
            return Ok(new { message = "user logged in successfully!", accessToken , refreshToken = refreshToken.Token });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var refreshToken = await _context.RefreshTokens.Include(rt => rt.User)
                                        .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshToken == null || refreshToken.ExpiryDate < DateTime.UtcNow || refreshToken.IsUsed || refreshToken.IsRevoked)
                return Unauthorized("Invalid or expired refresh token");

            var newAccessToken = _jwtService.GenerateToken(refreshToken.User);

            refreshToken.IsUsed = true;

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            newRefreshToken.UserId = refreshToken.UserId;

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new { accessToken = newAccessToken , refreshToken = newRefreshToken.Token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshToken == null)
                return NotFound("Invalid refresh token");

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            return Ok("Logged out sucessfully");
        }

   


        ////[Authorize]
        //[HttpPut("update-account-details/{Id}")]
        //public async Task<IActionResult> Update([FromBody] User user, int Id)
        //{
        //    var UserIdFromToken = int.Parse(User.FindFirst("UserId")?.Value);

        //    if (UserIdFromToken != Id)
        //    {
        //        return Forbid(); // ⬅️ Return 403 Forbidden if they try to update another user
        //    }

        //    var Exuser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == Id);
        //    if (Exuser == null)
        //        return NotFound();
        //    if (!BCrypt.Net.BCrypt.Verify(user.PasswordHash, Exuser.PasswordHash))
        //        return BadRequest(new { message = "Incorrect password, cannot update" });
        //    Exuser.Name = user.Name;
        //    Exuser.Email = user.Email;
        //    if (!string.IsNullOrEmpty(user.PasswordHash))
        //    {
        //        Exuser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        //    }
        //    _context.Users.Update(Exuser);
        //    await _context.SaveChangesAsync();
        //    return Ok(new { message = "user updated successfully!" });

        //}

    }
}
