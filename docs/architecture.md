# 🏗️ Atmos Architecture

### High Level Flow

- Repeated loop is numbered while unique startup and shutdown (or other) are _italicized_.

_Backend Core Functionality Startup_: The Orchestrator service startups up and does the following:
    - Based on a configuration setting, it initializes the `ISensorClient` (either `Rs485SensorClient` or `MockSensorClient`).
    - Initializes the `IAggregator` service, and decides to hydrate based on the last n readings. Where n is defined by the configuration by a least 1 minute and at maximum 1 day.
        - If there is not enough data to hydrate based on the configuration, a warning will be logged and the aggregator will start with an empty state. 

_React Frontend Startup (this would take place on refresh or new opening of the window)_: The React frontend connects to the SignalR hub and starts listening for updates.
    - The frontend will, by default, request the latest data from the server to populate the UI.
    - This will include the 12 hour table average, the latest reading, and any other relevant data.

### Sensor Polling Loop

1. **Awaken**: The `SensorPollingWorker` awakes from sleep every 10 seconds.
2. **Read Sensor**: It calls the `ISensorClient` to read data from the RS-485 sensor.
3. **Transform Data**: The raw sensor data is transformed into a `Reading` entity.
4. **Persist & Delegate**: The `Reading` entity is saved to the database and passed to the `IAggregator` in parallel using a fire and forget approach for the database operation.
5. **Calculate and Aggregate**: The `IAggregator` executes the core logic:
    - Cache: Adds the reading to an in-memory cache.
    - Updates Extremes: Updates the daily extremes for temperature, humidity, and dew point.
    - Calculates Averages: Computes the latest 1-minute rolling averages for temperature, humidity, and dew point.
    - Construct payload: Constructs an `UpdateDto` containing the latest statistics.
6. **Broadcast Update**: The `IAggregator` uses the `IHubContext` to broadcast the `UpdateDto` to all connected clients via SignalR.
7. **UI Handles update**: The React frontend receives the `UpdateDto` and updates the UI in real-time.
8. **Sleep**: The `SensorPollingWorker` goes back to sleep for 10 seconds before repeating the cycle.


_React triggered restart_: If the user decides to restart the polling service (for example, after a configuration change), the React frontend can send a request to the backend to stop and restart the `SensorPollingWorker`. This will gracefully shut down the current polling loop and start a new one with the updated configuration.


_React triggered dehydrate_: If the user wants to clear the in-memory cache of the `IAggregator`, they can send a request to the backend to dehydrate the aggregator. This will clear the in-memory state, allowing it to start fresh with new readings.

## Data & Infrastructure

### Data Models and Schema

**Reading Entity:**
- `UUID Id`
- `DateTime Timestamp`: Server-side timestamp of when the reading was taken. Will be indexed for fast querying.
- `double Temperature`: Temperature reading from the sensor.
- `double Humidity`: Humidity reading from the sensor.
- `double DewPoint`: Dew point calculated from the temperature and humidity.

**SensorData DTO:**
- `double Temperature`
- `double Humidity`
- `double DewPoint`


### Hardware Client

**Purpose:**
- Interface with the RS-485 sensor over a Serial Port connection.
**Implementation:**
- An interface `ISensorClient` that defines methods for reading data from the sensor.
- A concrete implementation `Rs485SensorClient` that uses the .NET `System.IO.Ports.SerialPort` class to manage the connection and communication. As well as a `MockSensorClient` for testing purposes.

**Responsibilities:**
- Manage the Serial Port connection.
- Sending the correct command bytes to the RS-485 sensor.
Reading and parsing the response from the sensor.
- Translating the raw response into the DTO (Data Transfer Object) format.
- Throwing custom, well-defined exceptions for any errors encountered during communication.


## Service

- Interfaces for `ISensorClient` belongs here. 

### Aggregator (Sensor Data Processing)

This service acts as a fast, in-memory cache and calculator for the real-time dashboard metrics. It holds a "hot" window of the most recent data to generate averages and other statistics without needing to query the database for every update.

**Purpose:**
- Aggregate and process sensor data for real-time metrics.
- Maintain short-term, in-memory storage of recent sensor readings.
- Trigger real-time updates to connected clients via SignalR.

**Implementation:**
- A singleton service that implements `IAggregator`.
- It must be a singleton to maintain state across requests.
- Uses a `ConcurrentQueue<SensorData>` to store the most recent readings.

**Responsibilities:**
- Maintain a thread-safe collection of recent sensor readings from the last ~5-10 minutes. This will be pruned periodically to keep the collection size manageable.
- Exposes a processing method that calculates averages, min/max, and other statistics from the recent readings.
- Cache the last pushed reading for quick access if the client disconnects and reconnects. (`LatestUpdate` property)
- Maintain a private, in memory state for daily extremes (min/max) for temperature, humidity, and dew point.
- Hydrate the latest reading and calculate the latest 1-minute rolling averages for temperature, humidity, and dew point.
- Maintain a private `DateTime? _currentDay` to track the current day for resetting daily extremes.
    - When the day changes, reset the min/max values for temperature, humidity, and dew point.
    - If a day changes the `_currentDay` is updated to the new day.
- Upon receiving new data, it will:
    1. Add the new reading to the internal cache
    2. Calculate the latest 1 minute rolling average for temperature, humidity, and dew point.
    2. Update the running min/max values of the day if required.
    4. Construct the `UpdateDto` (to be named later) which will house the latest statistics.
    5. Use the injected `IHubContext` to send the `UpdateDto` to all connected clients.

### Orchestrator (Sensor Polling Worker)

This is the main engine of the application. It is a long-running background service that orchestrates the entire polling and logging cycle.

**Purpose:**
- Run a continuous loop that polls the RS-485 sensor every 10 seconds.
**Implementation:**
- A class that implements `IHostedService` or `BackgroundService` to run in the background.
**Responsibilities:**
- Maintaining a precise 10-second polling timer using await and `Task.Delay`.
- Calling the `ISensorClient` to read data from the RS-485 sensor.
- Transforming raw sensor data into a `Reading` Entity. 
- Calling the `IReadingRepository` to save the reading to the database.
- Implementing top level error handling and retry logic to ensure the service remains resilient against transient errors.
**Dependencies:**
- `ISensorClient`: Interface for the hardware client that communicates with the RS-485 sensor.
- `IReadingRepository`: Interface for the data repository that handles reading storage.
- `ILogger<Orchestrator>`: For logging errors and important events during the polling process.


## SignalR Hub

This is the real-time communication endpoint for the application. The React frontend will connect to this hub to receive live updates without needing to poll the server.

**Purpose:**
- Provide a real-time communication channel for the frontend to receive updates.
- Send updates to connected clients whenever new sensor data is processed.

**Implementation**:
- A class AtmosHub that inherits from Microsoft.AspNetCore.SignalR.Hub.
- It will be largely passive; its primary role is to be the target for the AggregatorService. The hub itself won't contain significant logic.
- It can contain OnConnectedAsync and OnDisconnectedAsync overrides to log client connections and disconnections, which is useful for debugging.

**Responsibilities:**
- Receive `UpdateDto` from the AggregatorService and broadcast it to all connected clients.
- Maintain a list of connected clients for debugging purposes.
- Manage the lifecycle of client connections, including logging when clients connect or disconnect.

#### DTOs

**AverageDto:**
- `double? OneMinute`
- `double? FiveMinute`

**MinMaxDto:**
- `double Min`
- `double Max`

**ReadingDto:**
- `DateTime Timestamp`
- `double Temperature`
- `double Humidity`
- `double DewPoint`

**UpdateDto:**

- `ReadingDto LatestReading`
- `AverageDto TemperatureAverage`
- `AverageDto HumidityAverage`
- `AverageDto DewPointAverage`
- `MinMaxDto TemperatureMinMax`
- `MinMaxDto HumidityMinMax`
- `MinMaxDto DewPointMinMax`

## Web API

This is the RESTful API that the React frontend will use to fetch historical data and other information. It will also provide endpoints for administrative tasks like resetting the daily extremes.