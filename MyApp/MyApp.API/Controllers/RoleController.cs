using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;

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

        [HttpGet("")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            //var result = _myAppDbContext.Roles_Master.ToList();
            //var result = (from roles in _myAppDbContext.Roles_Master
            //             select roles).ToList();

            // var result = await _myAppDbContext.Roles_Master.ToListAsync();

            var result = await (from roles in _myAppDbContext.Roles_Master
                                select roles).ToListAsync();
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
        [Route("{name}")]
        public async Task<IActionResult> GetRoleByNameAndDescriptionFromQuerryStringAsync([FromRoute] string name, [FromQuery] string? description)
        {
            var result = await _myAppDbContext.Roles_Master
                .FirstOrDefaultAsync(x => x.Name == name && (String.IsNullOrWhiteSpace(description) && x.Description == description));
            return Ok(result);
        }
    }
}
