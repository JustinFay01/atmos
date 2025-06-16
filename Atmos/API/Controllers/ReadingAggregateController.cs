using AutoMapper;

using Domain.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/reading-aggregate")]
public class ReadingAggregateController : ControllerBase
{

    private readonly ILogger<ReadingAggregateController> _logger;
    private readonly IReadingAggregateRepository _aggregatorRepository;

    public ReadingAggregateController(ILogger<ReadingAggregateController> logger, IReadingAggregateRepository aggregatorRepository)
    {
        _logger = logger;
        _aggregatorRepository = aggregatorRepository;
    }


    [HttpGet]
    public async Task<IActionResult> GetReadingsAsync([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to)
    {
        _logger.LogInformation("Received request to GetReadingsAsync from: {From} | to: {To}", from, to);
        try
        {
            // Ensure from and to are in UTC
            var fromUtc = from?.ToUniversalTime() ?? DateTimeOffset.MinValue.ToUniversalTime();
            var toUtc = to?.ToUniversalTime() ?? DateTimeOffset.MaxValue.ToUniversalTime();

            var readings = await _aggregatorRepository.GetAsync(fromUtc, toUtc);

            return Ok(readings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting aggregated reading");
            return StatusCode(500, "Internal server error");
        }
    }

}
