# Atmos

## Overview

Atmos is a modern, robust application designed to read data from an RS-485 temperature sensor, provide real-time monitoring via a web-based dashboard, and log all historical data for later analysis. The system is designed for reliability and ease of deployment, running 24/7 to provide continuous climate insights.

The architecture is built on a decoupled client-server model, separating the core hardware interaction and data processing from the user-facing presentation layer. This ensures that each part of the system is specialized, testable, and maintainable.

## Core Features

*   **Real-Time Data Polling:** Communicates with an RS-485 sensor via a serial port every 10 seconds with high precision.
*   **Live Dashboard:** A dynamic web interface displaying:
    *   The most recent sensor readings (temperature, humidity, dew point).
    *   Aggregated 1-minute averages.
    *   A rolling 5-minute average.
    *   An hourly chart showing trends over the last 12 hours.
    *   Today's minimum and maximum climate readings.
*   **Historical Data Logging:** Every 10-second reading is permanently stored for historical analysis.
*   **History Viewer:** A dedicated page in the web interface to browse, paginate, and download historical data logs.
*   **Error Monitoring:** Integrated remote error logging to proactively identify and diagnose issues.

## Technology & Architecture

The system is composed of several key technological components that work together seamlessly.

### Backend (.NET)

The heart of the system is a .NET application responsible for all core logic. It acts as both a background service for hardware communication and a web server for data delivery.

*   **Hardware Communication:** A long-running background service directly interfaces with the serial port to send commands and parse data from the RS-485 sensor.
*   **Data Processing:** In-memory calculations are performed to generate the 1-minute, 5-minute, and hourly averages required for the live dashboard.
*   **Real-Time Communication (SignalR):** To achieve a true real-time experience on the dashboard, the backend uses **SignalR**. This technology allows the server to "push" new data to the web interface the instant it's available, eliminating the need for the browser to constantly poll for updates.
*   **Historical API (ASP.NET Core Web API):** The backend also hosts a Web API. This API provides endpoints for the frontend to query the historical database, supporting features like pagination and data downloading. It will also support methods to retrieve the averages based on a specific date/time range. This is incase the client disconnects from the live dashboard but still wants to access the latest data.
*   **Database ORM (Entity Framework Core):** To interact with the database in a safe and structured way, **Entity Framework (EF) Core** is used. It maps the sensor readings to database tables, handles database connections, and simplifies the process of writing data and querying historical logs.

### Frontend (React)

The user interface is a modern single-page application (SPA) built with **React**.

*   **Live Dashboard:** This page establishes a connection to the backend's SignalR hub. It listens for new data pushes and dynamically updates all relevant components—gauges, charts, and numerical readouts—without requiring a page refresh.
*   **History Page:** This page interacts with the backend's Web API. It allows the user to browse through all historical sensor readings, with pagination to handle large datasets efficiently. It will also include functionality to request and download logs for specific date ranges.

### Data Storage (PostgreSQL)

Instead of fragile text files, all historical readings are stored in a robust **PostgreSQL** database.~~

*   **Why PostgreSQL?** It is a powerful, open-source, and reliable relational database perfect for storing structured time-series data. It provides the ability to easily query, index, and manage vast amounts of historical information. The database will run in its own dedicated container for isolation and easy management.
    * Native support for date/time types and advanced querying capabilities.
    

~~Switching to SQLite.~~

~~Since this will be a desktop application, the database will be stored locally on the user's machine. This simplifies deployment and ensures that all data is accessible without needing a separate database server.~~

~~Following [Getting Started with EF Core](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)~~

### Logging & Error Monitoring (Sentry)

To ensure the system is running smoothly and to capture any potential problems, the backend is integrated with **Sentry**.

*   **Remote Error Logging:** If the backend application encounters an unhandled error (e.g., cannot connect to the serial port, fails to parse a message), that error is automatically captured and sent to a cloud-hosted Sentry instance.
*   **Proactive Maintenance:** This provides a remote dashboard to view application health, diagnose issues without needing direct access to the user's machine, and receive alerts on new or recurring problems. This is crucial for maintaining a service that needs to run unattended for long periods.

## Docs

- [Setup Guide](docs/setup.md)
- [🏗️ Architecture Overview](docs/architecture.md)
- [API Documentation](docs/api.md)
- [Tools](docs/tools.md)