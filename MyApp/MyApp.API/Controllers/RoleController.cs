using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs;
using MyApp.API.Entities;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly MyAppDbContext _myAppDbContext;
        public RoleController(MyAppDbContext myAppDbContext)
        {
            _myAppDbContext = myAppDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            //var result = _myAppDbContext.Roles_Master.ToList();
            //var result = (from roles in _myAppDbContext.Roles_Master
            //             select roles).ToList();

            var result = await _myAppDbContext.Roles_Master
                        .Select(x => new Role()
                        {
                            Id = x.Id,
                            Name = x.Name,
                        })
                        .ToListAsync();

            //OR

            var resultAlternate = await _myAppDbContext.Roles_Master
                                  .Select(x => new RoleDTO()
                                  {
                                      Id = x.Id,
                                      Title = x.Name,
                                  }).ToListAsync();

            //OR
            var resultAlternate2 = await _myAppDbContext.Roles_Master
                      .Select(x => new 
                      {
                          Id = x.Id,
                          Title = x.Name,
                      }).ToListAsync();



            var resultQuery = await (from roles in _myAppDbContext.Roles_Master
                                select roles).ToListAsync();

            //OR

            var resultQuerryAlternative = await (from roles in _myAppDbContext.Roles_Master
                                                 select new RoleDTO()
                                                 {
                                                        Id = roles.Id,
                                                        Title = roles.Name,
                                                 }).ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRolesByIdAsync([FromRoute] int id)
        {
            var result = await _myAppDbContext.Roles_Master.FindAsync(id);
            return Ok(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetRolesByNameAsync([FromRoute] string name)
        {
            var result = await _myAppDbContext.Roles_Master.Where(x => x.Name == name).FirstOrDefaultAsync();

            // For increasing performance a little we can remove the where and inside FirstOrDefault we can put the condition
            var resultOptimized = await _myAppDbContext.Roles_Master.FirstOrDefaultAsync(x => x.Name == name);

            return Ok(result);
        }

        //When its in route string then the parameter will be required to be in route string
        [HttpGet]
        [Route("{name}/{description}")]
        public async Task<IActionResult> GetRoleByNameAndDescriptionAsync([FromRoute] string name, [FromRoute] string description)
        {
            var result = await _myAppDbContext.Roles_Master.FirstOrDefaultAsync(x => x.Name == name && x.Description == description);
            return Ok(result);
        }

        //If we want to pass parameters to be required automatically then we can pass/take it from querry string or body 
        [HttpGet]
        [Route("GetRolesByNameAndDescription/{name}")]
        public async Task<IActionResult> GetRoleByNameAndDescriptionFromQuerryStringAsync([FromRoute] string name, [FromQuery] string? description)
        {
            //First or FirstOrDefault will return single object that matches the condition and is the first occurance
            //Single or SingleOrDefault will return single object that matches the condition and if more than one object matches it will throw an exception
            var result = await _myAppDbContext.Roles_Master
                .FirstOrDefaultAsync(x => x.Name == name && (String.IsNullOrWhiteSpace(description) && x.Description == description));

            //If we want to get all the records that matches the condition then we can use ToList or ToListAsync along with where
            var allResult = await _myAppDbContext.Roles_Master.Where(x => x.Name == name && (String.IsNullOrWhiteSpace(description) && x.Description == description)).ToListAsync();

            //There is also a way where ToList is used first and then the where is used, this is not recommended as it will fetch all the records from database and then filter it in memory
            var allResultNotRecommended = (await _myAppDbContext.Roles_Master.ToListAsync()).Where(x => x.Name == name && (String.IsNullOrWhiteSpace(description) && x.Description == description)).ToList();

            return Ok(result);
        }

        //Question: Fetch the data from Db based on Id [1,2,3] etc., where this [1,2,3] is dynamic, the user provides this data as a list or array etc.
        // Basically this is similar to SELECT * FROM Roles_Master WHERE Id IN (1,2,3)
        [HttpPost]
        [Route("GetRolesByIds")]
        public async Task<IActionResult> GetRolesByIds([FromBody] List<int> ids)
        {
            var result = await _myAppDbContext.Roles_Master.Where(x => ids.Contains(x.Id)).ToListAsync();

            return Ok(result);
        }
    }
}
