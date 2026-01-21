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
        /// <summary>
        /// This method is used to get the profile of the user based on username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{username}")]
        public async Task<IActionResult> GetUserByUserNameAsync([FromRoute] string username)
        {
            (long userId,string loggedInUserName) = User.GetUserIdAndUserName();

            bool isOwnProfile = string.Equals(
                loggedInUserName,
                username,
                StringComparison.OrdinalIgnoreCase
            );

            if (isOwnProfile)
            {
                var userProfile = await myNewDbContext.PrivateProfileView
                    .AsNoTracking()
                    .Where(u => u.UserName == username)
                    .SingleOrDefaultAsync();

                return userProfile == null
                           ? NotFound(new ResponseDTO
                           {
                               IsSuccess = false,
                               Message = "User Profile Not Found"
                           })
                           : Ok(userProfile);
            }
            else
            {
                var userProfile = await myNewDbContext.PublicProfileView
                    .AsNoTracking()
                    .Where(u => u.UserName == username)
                    .SingleOrDefaultAsync();

                return userProfile == null
                           ? NotFound(new ResponseDTO
                           {
                               IsSuccess = false,
                               Message = "User Profile Not Found"
                           })
                           : Ok(userProfile);
            }
        }

        /// <summary>
        /// This method is used to post user details/profile
        /// </summary>
        /// <param name="userDetailsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("profile")]
        public async Task<IActionResult> UpdateUserDetailsAsync([FromBody]UserDetailsDTO userDetailsDTO)
        {
            long userId = User.GetUserId();

            var user = await myNewDbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if(user == null)
                return NotFound(
                    new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    }
                );

            else if(!user.IsActive)
                return BadRequest(
                    new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Sorry! Your account is blocked."
                    }
                );

            var userDetail = await myNewDbContext.UserDetails.SingleOrDefaultAsync(ud => ud.UserId == userId);

            if(userDetail == null)
            {
                //create new user detail

                userDetail = new Domain.Entities.UserDetail
                {
                    UserId = userId,
                    FirstName = userDetailsDTO.FirstName,
                    LastName = userDetailsDTO.LastName,
                    DateOfBirth = userDetailsDTO.DOB,
                    Address = userDetailsDTO.Address,
                    PhoneNumber = userDetailsDTO.PhoneNumber,
                    CreatedBy = userId,
                };

                myNewDbContext.Add(userDetail);
            }
            else
            {
                // update User Details and change IsActive to true if its false.
                userDetail.FirstName = userDetailsDTO.FirstName;
                userDetail.LastName = userDetailsDTO.LastName;
                userDetail.PhoneNumber = userDetailsDTO.PhoneNumber;
                userDetail.DateOfBirth = userDetailsDTO.DOB;
                userDetail.Address = userDetailsDTO.Address;
                userDetail.UpdatedAt = DateTime.UtcNow;
                userDetail.UpdatedBy = userId;
                userDetail.IsActive = true;
            }

            await myNewDbContext.SaveChangesAsync();

            return Ok(
                new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Update successful"
                }
            );
        }
    }
}
