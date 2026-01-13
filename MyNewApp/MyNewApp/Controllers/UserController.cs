using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Domain.DTOs;
using MyNewApp.Domain.Entities;
using MyNewApp.Helpers;

namespace MyNewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(MyNewAppDbContext myNewDbContext) : ControllerBase
    {
        [HttpPost]
        [Route("update-user-details")]
        public async Task<IActionResult> UpdateUserDetails([FromBody]UserDetailsDTO userDetailsDTO)
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
                };

                myNewDbContext.Add(userDetail);
            }
            else
            {
                //update 
                //check if IsActive = 1
                if(userDetail.IsActive)
                {
                    userDetail.FirstName = userDetailsDTO.FirstName;
                    userDetail.LastName = userDetailsDTO.LastName;
                    userDetail.PhoneNumber = userDetailsDTO.PhoneNumber;
                    userDetail.DateOfBirth = userDetailsDTO.DOB;
                    userDetail.Address = userDetailsDTO.Address;
                }
                else
                {
                    //Inactive user detail or user
                    throw new Exception("Cannot update inactive user details.");
                }
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
