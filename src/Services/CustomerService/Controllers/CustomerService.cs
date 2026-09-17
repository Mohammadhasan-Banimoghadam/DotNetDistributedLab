using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        if (id != 1)
        {
            return NotFound();
        }

        return Ok(new
        {
            Id = 1,
            Name = "Mohammad",
            Email = "mohammad@test.com"
        });
    }

    //private static int _requestCount;

    //[HttpGet("{id:int}")]
    //public IActionResult Get(int id)
    //{
    //    if (id != 1)
    //    {
    //        return NotFound();
    //    }

    //    _requestCount++;

    //    Console.WriteLine(
    //        $"CustomerService - Attempt: {_requestCount}");

    //    if (_requestCount % 3 != 0)
    //    {
    //        return StatusCode(
    //            StatusCodes.Status500InternalServerError,
    //            "Temporary failure");
    //    }

    //    return Ok(new
    //    {
    //        Id = 1,
    //        Name = "Mohammad",
    //        Email = "mohammad@test.com"
    //    });
    //}
}