# 🏗️ Atmos Architecture

## Data & Service Layer

The backend consists of three primary logical components that work together: the Orchestrator, the Hardware Client, and the Data Repository.


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


### Data Repository

**Purpose:**
- Provide a clean interface for storing and retrieving sensor readings.
**Implementation:**
- An interface `IReadingRepository` that defines methods for saving and querying readings. 


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

### High Level Flow

1. The `SensorPollingWorker` main loop awakens every 10 seconds.
2. The worker class calls the `ISensorClient` to read data from the RS-485 sensor.
3. The `Rs485SensorClient` sends a command, awaits a response, parses the data, and returns a `SensorData` DTO.
4. The worker transforms the `SensorData` into a `Reading` entity via `AutoMapper`.
5. The worker calls the `IReadingRepository` to save the `Reading` entity to the database and adds the server-side timestamp.
6. The worker loops completes and calls `Tas.Delay()` to wait for the next cycle. 