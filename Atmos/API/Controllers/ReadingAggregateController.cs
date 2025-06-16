using AutoMapper;

using Domain.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/reading-aggregate")]
public class ReadingAggregateController : ControllerBase
{
    
    private readonly ILogger<ReadingAggregateController> _logger;
    private readonly IReadingAggregateRepository _aggregator;
    private readonly IMapper _mapper;

    public ReadingAggregateController(ILogger<ReadingAggregateController> logger, IReadingAggregateRepository aggregator, IMapper mapper)
    {
        _logger = logger;
        _aggregator = aggregator;
        _mapper = mapper;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetReadingsAsync([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
    {
        _logger.LogInformation("Getting aggregated reading");
        try
        {
            // Ensure from and to are in UTC
            var fromUtc = from.ToUniversalTime();
            var toUtc = to.ToUniversalTime();
            
            var readings = await _aggregator.GetAsync(fromUtc, toUtc);
            
            return Ok(readings);
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting aggregated reading");
            return StatusCode(500, "Internal server error");
        }
    }
    
}
