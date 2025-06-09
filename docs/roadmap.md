# 🚗 Atmos Roadmap

### Questions to Answer First

1.  **Configuration Management:**
    *   How will the backend know whether to use `Rs485SensorClient` or `MockSensorClient`? (Likely `appsettings.json` or environment variables, which is good for Docker).
    *   How will the serial port name/settings be configured?
    *   How will the `IAggregator` hydration window (1 min to 1 day) be configured?
    *   This isn't a new API endpoint, but something the backend needs to read at startup. Your `Orchestrator` startup mentions "based on a configuration setting."


### Phase 1: Core Backend POC & Data Flow

**Goal:** Establish the fundamental data pipeline: mock sensor -> processing -> storage -> real-time push. Verify backend logic independently.

1.  **Project Setup (.NET):**
    *   Create solution and projects:
        *   `Atmos.Domain`: Entities (`Reading`), Interfaces (`ISensorClient`, `IAggregator`, `IReadingRepository`), Core Services (Orchestrator, Aggregator).
        *   `Atmos.Infrastructure`: EF Core (`AppDbContext`, `ReadingRepository` implementation, Migrations), `Rs485SensorClient`, `MockSensorClient`.
        *   `Atmos.Presentation` (ASP.NET Core Web API/SignalR project): SignalR Hub (`AtmosHub`), DTOs, API Controllers (initially maybe just a placeholder).
        *   `Atmos.Core` (optional, for shared constants, enums, etc. if needed across projects).
2.  **Database Setup:**
    *   Define `Reading` entity.
    *   Implement `AppDbContext` with EF Core.
    *   Create initial EF Core migration.
    *   **Docker:** Setup `docker-compose.yml` for a PostgreSQL container.
    *   Configure connection string in `Atmos.Presentation` (`appsettings.json`).
3.  **Core Logic Implementation:**
    *   **`MockSensorClient`:** Implement `ISensorClient` to return mock `SensorData` DTOs on a timer or on demand.
    *   **`IReadingRepository`:** Implement saving `Reading` entities using EF Core.
    *   **`IAggregator`:**
        *   Implement in-memory caching (e.g., `ConcurrentQueue<ReadingDto>`).
        *   Implement logic for 1-minute, 5-minute averages, and daily extremes (initially, can be simplified).
        *   Implement logic to construct `UpdateDto`.
        *   Integrate `IHubContext<AtmosHub>` for broadcasting.
    *   **`Orchestrator` (`BackgroundService`):**
        *   Implement polling loop (`Task.Delay`).
        *   Inject `ISensorClient` (configurable to use `MockSensorClient`).
        *   Inject `IReadingRepository` and `IAggregator`.
        *   Implement the flow: Read -> Transform -> Persist (fire-and-forget) -> Delegate to Aggregator.
        *   Basic logging (`ILogger`).
4.  **SignalR Hub (`AtmosHub`):**
    *   Define the hub class.
    *   Implement `OnConnectedAsync` / `OnDisconnectedAsync` for logging (optional but useful).
5.  **Initial DTOs:**
    *   Define `ReadingDto`, `AverageDto`, `MinMaxDto`, `UpdateDto`.
6.  **Testing & Verification (Phase 1):**
    *   Run the backend application.
    *   **Postman (WebSocket Client):** Connect to the SignalR `AtmosHub` endpoint.
    *   Verify `UpdateDto` messages are received every 10 seconds.
    *   Check PostgreSQL database to confirm `Reading` entities are being saved.
    *   Manually inspect logs for any errors.

---

### Phase 2: MVP - End-to-End Visibility & Deployment Basics

**Goal:** Get data from a real (or robustly mocked) sensor, through a Dockerized backend, to a basic web UI. Solve key deployment challenges.

1.  **Unit Testing Foundation:**
    *   Setup `Atmos.Tests` project (xUnit/NUnit/MSTest).
    *   Write initial unit tests for `Aggregator` logic (e.g., average calculations, extreme updates).
    *   Write unit tests for `MockSensorClient`.
2.  **Backend Dockerization & Real Sensor Integration:**
    *   Create `Dockerfile` for `Atmos.Presentation`.
    *   Update `docker-compose.yml` to build and run the backend container.
    *   **Crucial:** Configure Docker to map the host's serial port to the backend container.
    *   Implement `Rs485SensorClient` (actual serial port communication logic).
    *   **Thoroughly test** serial port communication *through the Docker container*. This is a high-risk area.
    *   Add configuration to switch between `MockSensorClient` and `Rs485SensorClient` easily (e.g., via environment variable in `docker-compose.yml`).
3.  **Database Migrations in Docker:**
    *   Research and implement a strategy for applying EF Core migrations automatically when the backend container starts (e.g., in `Program.cs` or an entrypoint script). Test this thoroughly.
4.  **React Frontend POC:**
    *   Create a basic React application (e.g., using `create-react-app`).
    *   Install SignalR client library (`@microsoft/signalr`).
    *   Implement basic connection to the backend's `AtmosHub`.
    *   Display the raw `UpdateDto` data received via SignalR (no styling needed, just verify data flow).
    *   If running React dev server and backend in Docker, configure CORS on the backend.
5.  **REST Endpoint: Initial Dashboard State:**
    *   **Implement `GET /api/dashboard/state` in `Atmos.Presentation`.**
        *   This controller will need access to the `IAggregator` (for `latestUpdate`) and `IReadingRepository` (or a new service) to calculate `hourlyAveragesLast12Hours`.
    *   React POC: On load/connect, call this endpoint to populate the initial view before SignalR updates start or if it missed initial pushes.
6.  **Sentry Integration:**
    *   Integrate Sentry SDK into `Atmos.Presentation` for backend error logging.
    *   Integrate Sentry SDK into the React POC for frontend error logging.
    *   Test by intentionally throwing an error in both backend and frontend.
7.  **Refine `docker-compose.yml`:**
    *   Ensure backend, database (and potentially a basic Nginx for React static files if not using dev server) can be brought up with `docker-compose up`.

---

### Phase 3: Feature Completeness & Polish

**Goal:** Implement all core features described in the README, create a user-friendly interface, and prepare for deployment.

1.  **React Frontend Development - Live Dashboard:**
    *   Design and style the Live Dashboard page.
    *   Implement components for:
        *   Most recent readings (temperature, humidity, dew point).
        *   1-minute and 5-minute averages.
        *   Today's min/max readings.
        *   Hourly chart for the last 12 hours (using data from `GET /api/dashboard/state` and potentially updated by SignalR if you decide to push hourly aggregates too, though less critical).
    *   Ensure all components update dynamically based on SignalR `UpdateDto` pushes.
2.  **REST Endpoints for Historical Data:**
    *   Implement `GET` (with pagination, filtering, sorting).
    *   Implement `GET` (CSV export).
    *   Implement `GET` (for querying averages over custom ranges/intervals).
3.  **React Frontend Development - History Viewer:**
    *   Design and style the History page.
    *   Implement browsing and pagination for historical readings (using `GET`).
    *   Implement functionality to select date ranges and download data (using `GET`).
    *   (Optional Advanced) Implement a way to view/chart custom aggregate data using `GET`.
4.  **Administrative REST Endpoints & Frontend Integration (if UI controls are desired):**
    *   Implement `POST`.
    *   Implement `POST`.
    *   Implement `POST`.
    *   If these are to be triggered from the UI, add simple buttons/forms in a settings area of the React app.
    *   **Security:** If these admin endpoints are exposed, consider adding basic authentication/authorization.
5.  **Configuration Endpoints (Optional):**
    *   If you decide the frontend needs to view or modify parts of the backend configuration:
        *   Implement `GET`.
        *   Implement `PUT`.
        *   **Security:** These definitely need to be secured.
6.  **Finalize `IAggregator` Hydration:**
    *   Implement the startup hydration logic for the `IAggregator` based on configuration and historical data.
7.  **Documentation:**
    *   Review and update `README.md`.
    *   Write `docs/setup.md`.
    *   Write basic `docs/api.md` for the REST endpoints.
8.  **Testing & Refinement:**
    *   More comprehensive unit tests.
    *   Integration tests (e.g., testing API endpoints with an in-memory test server or against a test DB).
    *   End-to-end testing (manual or automated if you have the tools/time).
    *   Test resilience (e.g., what happens if DB is temporarily down, sensor disconnects).
    *   Test Docker deployment thoroughly on a clean system.

---