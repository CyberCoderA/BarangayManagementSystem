using Microsoft.AspNetCore.Mvc;

namespace BarangayManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("status")]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Barangay Management System API is working!"
            });
        }
    }
}