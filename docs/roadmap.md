# 🚗 Atmos Roadmap

### Phase 1: Core Backend POC & Data Flow

**Goal:** Establish the fundamental data pipeline: mock sensor -> processing -> storage -> real-time push. Verify backend logic independently.

1~~.  **Project Setup (.NET):**~~
    *   Create solution and projects:
        *   `Atmos.Domain`: Entities (`Reading`), Interfaces (`ISensorClient`, `IAggregator`, `IReadingRepository`), Core Services (Orchestrator, Aggregator).
        *   `Atmos.Infrastructure`: EF Core (`AppDbContext`, `ReadingRepository` implementation, Migrations), ~~`Rs485SensorClient`~~ **Moving to phase 2**, `MockSensorClient`.
        *   `Atmos.Presentation` (SignalR project): SignalR Hub (`DashboardHub`) and DTOs
2.  ~~**Database Setup:**~~
    *   Define `Reading` entity.
    *   Implement `AppDbContext` with EF Core.
    *   Create initial EF Core migration.
    ~~*   **Docker:** Setup `docker-compose.yml` for a PostgreSQL container.~~
    ~~*   Configure connection string in `Atmos.Presentation` (`appsettings.json`).~~
    * Set up SQLite database for local storage.
3.  ~~**Core Logic Implementation:**~~
    *   **`MockSensorClient`:** Implement `ISensorClient` to return mock `SensorData` DTOs on a timer or on demand.
    *   **`IReadingRepository`:** Implement saving `Reading` entities using EF Core.
    *   **`IAggregator`:**
        *   Implement in-memory caching (e.g., `ConcurrentQueue<ReadingDto>`).
        *   Implement logic for 1-minute, 5-minute averages, and daily extremes (initially, can be simplified).
        *   Implement logic to construct `UpdateDto`.
        *   Integrate `IHubContext<AtmosHub>` for broadcasting.
        *   Unit tests for calculation logic (e.g., average calculations, extreme updates).
        *   MinMax Reset rule at midnight in a given timezone. 
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
    ~~*   Define `ReadingDto`, `AverageDto`, `MinMaxDto`, `UpdateDto`.~~
    ~~* Map MetricAggregates (and their Metric) to value types for simplified interface with frontend.~~
6.  **Testing & Verification (Phase 1):**
    *   Run the backend application.
    ~~*   **Postman (WebSocket Client):** Connect to the SignalR `AtmosHub` endpoint.~~ --> **Switched to client.ts because postman doesn't support SignalR.**
    *   Verify `UpdateDto` messages are received every 10 seconds.
    ~~*   Check PostgreSQL database to confirm `Reading` entities are being saved.~~
    *   Manually inspect logs for any errors.

---

### Phase 2: MVP - End-to-End Visibility & Deployment Basics

**Goal:** Get data from a real (or robustly mocked) sensor, through a Dockerized backend, to a basic web UI. Solve key deployment challenges.

~~1.  **Unit Testing Foundation:**
    *   Setup `Atmos.Tests` project (xUnit/NUnit/MSTest).
    *   Write initial unit tests for `Aggregator` logic (e.g., average calculations, extreme updates).
    *   Write unit tests for `MockSensorClient`.~~ **Moved to Phase 1.**
2.  **Real Sensor Integration:**
    *   Implement `Rs485SensorClient` (actual serial port communication logic).
    *   Add configuration to switch between `MockSensorClient` and `Rs485SensorClient` ~~easily (e.g., via environment variable in `docker-compose.yml`).~~
~~3.  **Database Migrations in Docker:**~~
    ~~*   Research and implement a strategy for applying EF Core migrations automatically when the backend container starts (e.g., in `Program.cs` or an entrypoint script). Test this thoroughly.~~
3. ~~**Postgres Database**~~
    *   Implement routine backups of the Postgres database.
    *   Define strategy for handling database migrations in Postgres.
    *   Stop reading sensor if database isn't saving readings.
5. ~~**React Frontend POC:**~~
    *   Create a basic React application (e.g., using `create-react-app`).
    *   Install SignalR client library (`@microsoft/signalr`).
    *   Implement basic connection to the backend's `AtmosHub`.
    *   Display the raw `UpdateDto` data received via SignalR (no styling needed, just verify data flow).
    *  Set up C# Static Files to host the React app in `Atmos.Presentation`.
    * Basic frontend displays:
      * Three dials for current temperature, humidity, and dew point.
      * Today's min/max readings.
      * A realtime clock showing current time in the configured timezone.
      * A simple line chart showing the last hour of temperature readings (updated in real-time).
        * For now the this will reset on refresh, later we will implement a cache of these readings in the backend.
      * A raw readings table showing the last 10 readings received.
    ~~*   If running React dev server and backend in Docker, configure CORS on the backend.~~
5.  **REST Endpoint: Initial Dashboard State:**
    *   Fetch historical data for a given date and time range. 
    ~~*   Aggregate the data into the 5 minute and 1 minute averages (or potentially store these in the database).~~
      * Must store these in the database for historical queries.
    * Select file format implementation (e.g., CSV, TXT)
    *   Convert the UTC time to the local timezone of the user (if specified in the request).
    * Have the option to unique 5 minute averages
6.  ~~**Sentry Integration:**~~
    *   Integrate Sentry SDK into `Atmos.Presentation` for backend error logging.
    *   Integrate Sentry SDK into the React POC for frontend error logging.
    *   Test by intentionally throwing an error in both backend and frontend.
    ~~*   Determine where logs are saved to, do they go to sentry? or do we save local files?~~
~~7.  **Refine `docker-compose.yml`:**~~
    ~~*   Ensure backend, database (and potentially a basic Nginx for React static files if not using dev server) can be brought up with `docker-compose up`.~~

---

### Phase 3: Feature Completeness & Polish

**Goal:** Implement all core features described in the README, create a user-friendly interface, and prepare for deployment.

1.  ~~**React Frontend Development - Live Dashboard:**~~
    *   Design and style the Live Dashboard page.
    *   Implement components for:
        *   Most recent readings (temperature, humidity, dew point).
        *   1-minute and 5-minute averages.
        *   Today's min/max readings.
        *   Hourly chart for the last 12 hours (using data from `GET /api/dashboard/state` and potentially updated by SignalR if you decide to push hourly aggregates too, though less critical).
    *   Ensure all components update dynamically based on SignalR `UpdateDto` pushes.
2.  ~~**REST Endpoints for Historical Data:**~~
    *   Implement `GET` (with pagination, filtering, sorting).
    *   Implement `GET` (CSV export).
    *   Implement `GET` (for querying averages over custom ranges/intervals).
3.  ~~**React Frontend Development - History Viewer:**~~
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
~~~~6.  **Finalize `IAggregator` Hydration:**~~
    ~~*   Implement the startup hydration logic for the `IAggregator` based on configuration and historical data.~~~~
7.  **Documentation:**
    *   Review and update `README.md`.
    *   Write `docs/setup.md`.
    *   Write basic `docs/api.md` for the REST endpoints.
8.  **Testing & Refinement:**
    *   More comprehensive unit tests.
    *   Integration tests (e.g., testing API endpoints with an in-memory test server or against a test DB).
    *   End-to-end testing (manual or automated if you have the tools/time).
    *   Test resilience (e.g., what happens if DB is temporarily down, sensor disconnects).
    *   Test Docker deployment thoroughly on a clean system.'

### Phase 3.5: Installer 
#### **Phase 0: Project Setup & Dependencies**

This phase covers setting up the C# console project for the launcher itself.

*   **[ ] 1. Create Project:**
    *   Create a new `.NET Console Application` project named `Atmos.Launcher`.
    *   Ensure the project is configured to be published as a **self-contained** single executable.

*   **[ ] 2. Add NuGet Packages:**
    *   **[ ] `Octokit.NET`:** For interacting with the GitHub API to check for new releases.
    *   **[ ] `Spectre.Console`:** (Highly Recommended) For creating a much nicer user interface with prompts, spinners, and status tables. This will significantly improve the user experience.
    *   Other core dependencies will be from the .NET SDK itself (`System.Text.Json`, `System.IO.Compression`, `System.Net.Http`).

*   **[ ] 3. Define Core Helper Functions:**
    *   Create a utility class for running external processes (like `docker`, `sc.exe`, and the migration tool) using `System.Diagnostics.Process`. This class should be able to capture standard output, standard error, and exit codes.

---

#### **Phase 1: First-Time Installation Logic**

This is the workflow when the launcher detects it is being run in a new location.

*   **[ ] 1. Prerequisite Checks:**
    *   **[ ] Docker:** Execute `docker version`. If it fails, halt the installation and instruct the user to install and start Docker Desktop.
    *   **[ ] .NET Runtime:** Check for the required ASP.NET Core Runtime version needed by the main Atmos service. If not found, halt and provide a download link.

*   **[ ] 2. User Interaction & Setup:**
    *   **[ ] Prompt for Path:** Ask the user for the desired installation directory (e.g., `C:\Atmos`). Default to a sensible location.
    *   **[ ] Create Directory Structure:** Create the target directory.
    *   **[ ] Copy Application Files:** Copy the bundled `app/` folder and `docker-compose.yml` into the new directory.

*   **[ ] 3. Configuration:**
    *   **[ ] Generate DB Password:** Create a cryptographically secure random string for the Postgres password.
    *   **[ ] Create `.env` file:** Write `POSTGRES_PASSWORD=<generated_password>` to a new `.env` file in the installation root (`C:\Atmos\.env`).
    *   **[ ] Create `appsettings.json`:** Generate the `appsettings.production.json` file inside the `app` folder, injecting the correct connection string using `localhost`, the default port, and the generated password.

*   **[ ] 4. System Deployment:**
    *   **[ ] Start Database:** Run `docker-compose up -d` using the `docker-compose.yml` in the target directory. Wait for it to complete and check for errors.
    *   **[ ] Run Migrations:** Execute the `Atmos.Migration.exe` tool, passing the connection string as an argument. Check for a successful exit code.

*   **[ ] 5. Finalizing Installation:**
    *   **[ ] Create State File:** Create a `launcher.json` or `atmos.json` file in the installation root. Store the current version in it (e.g., `{ "InstalledVersion": "1.0.0" }`).
    *   **[ ] Register as a Service:** (Windows Focus) Execute `sc.exe create AtmosService binPath="C:\Atmos\app\Atmos.Service.exe" start=auto` to register the main application as a Windows Service that starts on boot.
    *   **[ ] Start Service:** Execute `sc.exe start AtmosService`.
    *   **[ ] Create Shortcut:** Create a desktop shortcut pointing to the `atmos-launcher.exe` in the installation directory.
    *   **[ ] Display Success Message:** Inform the user the installation is complete and the dashboard is ready.

---

#### **Phase 2: Management & Update Logic**

This is the workflow when the launcher is executed from an existing installation directory.

*   **[ ] 1. Check System Status:**
    *   Check if the Postgres Docker container is running (`docker ps`).
    *   Check if the `AtmosService` Windows Service is running (`sc.exe query AtmosService`).

*   **[ ] 2. Check for Updates:**
    *   Read the `InstalledVersion` from the local `launcher.json`.
    *   Use `Octokit.NET` to query the latest release from your GitHub repository.
    *   Compare versions. If a newer version is available, store its download URL and tag name.

*   **[ ] 3. Display Management Menu:**
    *   Use `Spectre.Console` to show a menu with the system status and options:
        *   `[O]pen Dashboard` (Launches `http://localhost:5000` in the default browser)
        *   `[S]top / Start Service`
        *   `[R]estart Service`
        *   `[U]pdate to vX.X.X` (This option only appears if an update is available)
        *   `[L]ogs` (Tails the application log file)
        *   `[E]xit`

*   **[ ] 4. Implement Update Workflow (for option [U]):**
    *   **[ ] Stop Service:** `sc.exe stop AtmosService`.
    *   **[ ] Download & Extract:** Download the new release `.zip` asset from GitHub. Unzip its `app/` contents, overwriting the existing files in `C:\Atmos\app`.
    *   **[ ] Run Migrations:** Execute the migration tool again (it's safe and idempotent).
    *   **[ ] Update State File:** Update the `InstalledVersion` in `launcher.json` to the new version.
    *   **[ ] Start Service:** `sc.exe start AtmosService`.
    *   **[ ] Report Success.**

---

#### **Phase 3: Packaging for Release**

This defines the final structure of the asset you will upload to GitHub Releases.

*   **[ ] 1. Define Release ZIP Structure:**
    *   The final `atmos-vX.X.X.zip` should contain:
        ```
        /atmos-launcher.exe         <-- Your compiled, self-contained launcher
        /docker-compose.yml         <-- The docker-compose file for Postgres
        /app/                       <-- A folder containing the main application
            /Atmos.Service.exe
            /Atmos.Migration.exe
            /wwwroot/
            /... (all other required DLLs and files)
        ```
*   **[ ] 2. Create Build Script:**
    *   Automate the process of publishing the Launcher, publishing the Service, gathering the files, and creating the final ZIP archive with the correct structure. The core command for your launcher will be `dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true`.

### Phase 4: Stretch Goals 

1. ~~**Timing and Performance:**~~
   * Handle computer sleep/wake events gracefully.
   ~~* Implement a more robust retry mechanism for sensor reads.~~
   ~~* Implement a start delay 'cushion.' Currently, if the orchestrator starts to close to: 00.00, then the reading may occur at :59.99.~~
   ~~* Fix download query for performance. Consider server-side streaming or pagination for large datasets.~~
2. **Maintenance and Monitoring:**
   * Monthly dependabot updates to check for outdated dependencies.
