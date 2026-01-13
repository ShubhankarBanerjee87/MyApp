using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyNewApp.Data;
using MyNewApp.Domain.DTOs;
using MyNewApp.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace MyNewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _passwordHasher; 
        private readonly MyNewAppDbContext myNewDbContext;

        public AuthController(IConfiguration config, MyNewAppDbContext myNewDbContext, IPasswordHasher<User> passwordHasher)
        {
            _config = config;
            this.myNewDbContext = myNewDbContext;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// This method handles user registration.
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [EnableRateLimiting("SignUp")]
        [AllowAnonymous]
        [HttpPost]
        [Route("signup")]
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

        /// <summary>
        /// This method handles user login, verifies credentials, and generates JWT and refresh tokens.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [EnableRateLimiting("Login")]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            int refreshTokenExpirInDaysy = int.Parse(_config["Jwt:RefreshExpiryInDays"]!);
            var email = login.Email.Trim().ToLower();

            //get the user along with roles
            var user = await myNewDbContext.Users
                .Include(u => u.UserRoles)!
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
            {
                return Unauthorized(new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid email."
                });
            }

            //check the password
            var verifyPasswordResult = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);

            if (verifyPasswordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid password."
                });
            }

            var roles = user.UserRoles?
            .Where(ur => ur.IsActive)
            .Select(ur => ur.Role!.RoleTitle)
            .ToList();

            //generate jwt token
            var accessToken = GenerateToken(user, roles);


            //Generate refresh token
            var refreshTokenValue = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirInDaysy),
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            myNewDbContext.RefreshTokens.Add(refreshToken);
            await myNewDbContext.SaveChangesAsync();

            var loginResponse = new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles!,
                AcceessTokenExpiresIn = int.Parse(_config["Jwt:ExpiryMinutes"]!)
            };

            return Ok(new ResponseDTO
            {
                IsSuccess = true,
                Message = "Login successful.",
                Data = loginResponse
            });
        }

        #region Private methods
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

        // JWT token generator
        private string GenerateToken(User user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Secure refresh token generator
        private static string GenerateRefreshToken()
        {
            var randomBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        #endregion
    }
}
