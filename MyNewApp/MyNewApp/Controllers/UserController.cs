using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyNewApp.Data;

namespace MyNewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(MyNewAppDbContext myNewDbContext) : ControllerBase
    {
        [HttpPost]
        [Route("SignUp")]
        public Task<IActionResult> SignUp([FromBody])
        {

        }
    }
}
