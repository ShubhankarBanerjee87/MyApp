using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Domain.DTOs;
using MyNewApp.Helpers;

namespace MyNewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(MyNewAppDbContext myNewDbContext) : ControllerBase
    {
        [HttpPost]
        [Route("Update-UserDetails")]
        public async Task<IActionResult> UpdateUserDetails([FromBody]UserDetailsDTO userDetails)
        {
            long userId = User.GetUserId();

            var detail = new Domain.Entities.UserDetail
            {
                UserId = userId,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                DateOfBirth = userDetails.DOB,
                Address = userDetails.Address,
                PhoneNumber = userDetails.PhoneNumber,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
            };

            myNewDbContext.Add(detail);
            await myNewDbContext.SaveChangesAsync();

        }

    }
}
