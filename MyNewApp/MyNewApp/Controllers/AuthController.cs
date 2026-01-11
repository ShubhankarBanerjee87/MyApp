using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Domain.DTOs;
using MyNewApp.Domain.Entities;


namespace MyNewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(MyNewAppDbContext myNewDbContext, IPasswordHasher<User> _passwordHasher) : ControllerBase
    {
        [EnableRateLimiting("SignUp")]
        [AllowAnonymous]
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto register)
        {
            ResponseDTO response = new ResponseDTO();
            try
            {
                var email = register.Email.Trim().ToLower();
                var userName = string.IsNullOrWhiteSpace(register.UserName)
                    ? await GenerateUserName(email)
                    : register.UserName.Trim();

                var newUser = new Domain.Entities.User()
                {
                    Email = email,
                    UserName = userName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UserRoles = new List<Domain.Entities.UserRole>()
                    {
                        new Domain.Entities.UserRole()
                        {
                            RoleId = 5 // Assigning 'Guest' role by default
                        }
                    }
                };

                newUser.Password = _passwordHasher.HashPassword(newUser, register.Password);

                myNewDbContext.Users.Add(newUser);
                await myNewDbContext.SaveChangesAsync();

                response = new ResponseDTO()
                {
                    IsSuccess = true,
                    Message = "User registered successfully.",
                };

                return Ok(response);
            }
            catch (DbUpdateException ex)
            {
                response = new ResponseDTO()
                {
                    IsSuccess = false,
                };

                //Handle Unique constraint violations
                if (ex.InnerException?.Message.Contains("IX_Users_Email") == true)
                {
                    response.Message = "Email is already registered.";
                    return Conflict(response);
                }

                if (ex.InnerException?.Message.Contains("IX_Users_UserName") == true)
                {
                    response.Message = "Username is already taken.";
                    return Conflict(response);
                }

                throw; // Re-throw if it's a different exception
            }
        }

        private async Task<string> GenerateUserName(string email)
        {

            var baseName = email.Split('@')[0];
            var userName = baseName;
            int counter = 1;

            while (await myNewDbContext.Users.AnyAsync(u => u.UserName == userName))
            {
                userName = $"{baseName}_{counter}";
                counter++;
            }

            return userName;
        }
    }
}
