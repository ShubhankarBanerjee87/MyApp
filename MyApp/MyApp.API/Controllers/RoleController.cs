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
    }
}
