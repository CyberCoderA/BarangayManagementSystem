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

        [HttpGet("database-status")]
        public IActionResult GetDatabaseStatus([FromServices] ApplicationDbContext dbContext)
        {
            try
            {
                // Attempt to connect to the database
                dbContext.Database.CanConnect();
                return Ok(new { message = "Database connection is successful!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database connection failed.", error = ex.Message });
            }
        }
    }
}