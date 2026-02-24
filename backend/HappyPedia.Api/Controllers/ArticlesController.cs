using Microsoft.AspNetCore.Mvc;

namespace HappyPedia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new[] { "it works" });
}