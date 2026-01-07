using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MyApp.API.Data;
using MyApp.API.DTOs;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(MyAppDbContext _myAppDbContext) : ControllerBase
    {
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddUser(UserDTO user)
        {

            //check if user with same email or username exists
            var existingUser = await _myAppDbContext.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email || u.UserName == user.UserName);

            if (existingUser != null)
            {
                //User with same email or username exists
                return Conflict("User with same email or username already exists.");
            }

            //check if roles exist

            var roles = await _myAppDbContext.Roles_Master
                .Where(r => user.RoleIds.Contains(r.Id))
                .ToListAsync();

            if (roles.Count != user.RoleIds.Count)
            {
                //Roles provided do not all exist
                return BadRequest("One or more roles provided do not exist.");
            }

            var newUser = new Entities.User()
            {
                Email = user.Email,
                UserName = user.UserName,
                Password = user.Password,
            };

            _myAppDbContext.Users.Add(newUser);
            await _myAppDbContext.SaveChangesAsync();

            var userRoles = roles.Select(r => new Entities.UserRole
            {
                UserId = newUser.Id,
                RoleId = r.Id
            }).ToList();

            _myAppDbContext.UserRoles.AddRange(userRoles);
            await _myAppDbContext.SaveChangesAsync();


            return Ok();
        }

        [HttpPost]
        [Route("AddInSingleHit")]
        public async Task<IActionResult> AddDataInSingleHit(UserDTO user)
        {
            var checkExistingUser = await _myAppDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email || u.UserName == user.UserName);

            if(checkExistingUser != null)
            {
                return Conflict("User Exists");
            }

            var checkRoles = await _myAppDbContext.Roles_Master.Where(r => user.RoleIds.Contains(r.Id)).ToListAsync();

            if(checkRoles.Count != user.RoleIds.Count)
            {
                return Conflict("Roles doesnt exists");
            }


            var newUser = new Entities.User()
            {
                Email = user.Email,
                UserName = user.UserName,
                Password = user.Password,
                UserRoles = user.RoleIds
                .Select(roleId => new Entities.UserRole
                {
                    RoleId = roleId
                })
                .ToList()
            };

            _myAppDbContext.Users.Add(newUser);
            await _myAppDbContext.SaveChangesAsync();

            return Ok(newUser);
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var user = await _myAppDbContext.Users.Where(u => u.Email == login.Email && u.Password == login.Password)
                .Select(u =>
                new
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName
                }).ToListAsync();

            //Now if i want to get the user details along with join then

            return Ok(user);
        }

    }
}
