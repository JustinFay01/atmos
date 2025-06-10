# Project Structure

```
Atmos.sln
└── src/
├── Atmos.Domain/                  // Core business logic and entities. No dependencies.
│   ├── Entities/
│   │   └── Reading.cs             // The core Reading entity.
│   └── Interfaces/
│       └── IReadingRepository.cs  // The persistence contract (the "port").
│
├── Atmos.Application/             // Application logic, use cases, and orchestration.
│   ├── DTOs/                      // Data Transfer Objects for APIs and hubs.
│   │   ├── ReadingDto.cs
│   │   └── UpdateDto.cs
│   ├── Interfaces/
│   │   ├── ISensorClient.cs       // Contract for any sensor hardware.
│   │   └── IAggregator.cs         // Contract for the aggregation service.
│   ├── Services/
│   │   └── AggregatorService.cs   // Implementation of IAggregator.
│   └── Workers/
│       └── SensorPollingWorker.cs // The IHostedService that runs the main loop.
│
├── Atmos.Infrastructure/          // Implementations of external concerns (database, hardware).
│   ├── Hardware/
│   │   └── Sensors/
│   │       ├── Rs485SensorClient.cs // Real implementation of ISensorClient.
│   │       └── MockSensorClient.cs  // Mock implementation for testing.
│   └── Persistence/
│       ├── AppDbContext.cs        // The Entity Framework DbContext.
│       └── Repositories/
│           └── ReadingRepository.cs // EF Core implementation of IReadingRepository (the "adapter").
│
└── Atmos.Api/                     // The entry point (Web API, SignalR, DI configuration).
├── Controllers/
│   └── ReadingsController.cs  // REST API for historical data, etc.
├── Hubs/
│   └── AtmosHub.cs            // SignalR hub implementation.
├── appsettings.json
└── Program.cs                 // DI container setup, pipeline configuration.
```