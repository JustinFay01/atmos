using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application;

public class SensorPollingWorker : BackgroundService
{
    private readonly ILogger<SensorPollingWorker> _logger;
    private readonly ISensorClient _sensorClient;
    private readonly IAggregator _aggregator;
    private readonly IReadingRepository _readingRepository;
    private readonly IMapper _mapper;

    public SensorPollingWorker(ILogger<SensorPollingWorker> logger, IAggregator aggregator, ISensorClient sensorClient, IMapper mapper, IReadingRepository readingRepository)
    {
        _logger = logger;
        _aggregator = aggregator;
        _sensorClient = sensorClient;
        _mapper = mapper;
        _readingRepository = readingRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("SensorPollingWorker is starting.");
        
        // TODO: Add error handling
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
           
            var latestReadingDto = await _sensorClient.GetReadingAsync(stoppingToken);
            _logger.LogDebug("Latest reading received: {reading}", latestReadingDto);
            
            var readingEntity = _mapper.Map<Reading>(latestReadingDto);
            
            var saveTask = _readingRepository.CreateAsync(readingEntity, stoppingToken);
            var processTask = _aggregator.ProcessReadingAsync(readingEntity);
            await Task.WhenAll(saveTask, processTask);    
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}