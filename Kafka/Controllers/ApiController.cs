using Microsoft.AspNetCore.Mvc;

namespace Kafka.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    //TODO
    //Create Handler to send as consume messages
    private readonly ILogger<ApiController> _logger;

    public ApiController(ILogger<ApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "SendMessage")]
    public async Task<IActionResult> SendMessages(/*Create the Contract*/CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}