using Microsoft.AspNetCore.Mvc;
using MyCompany.Data;

namespace MyCompany.Api.Controllers;

[Route("api/health")]
[ApiController]
public class HealthController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    [HttpGet]
    public async Task<IActionResult> CheckHealth()
    {
        try
        {
            // Do any operation on the DB - if not connected this will throw
            await _dbContext.Database.CanConnectAsync();
            return Ok(new { status = "Healthy", dbStatus = "Connected", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "Unhealthy", dbStatus = "Error", message = ex.Message, timestamp = DateTime.UtcNow });
        }
    }
}