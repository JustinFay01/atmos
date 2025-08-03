# Setup 

### Production Installation

#### Prerequisites

- .NET 9 SDK or later
- Docker
- Docker Compose

#### Steps

1. Open a command prompt or terminal in administrator mode by right-clicking and selecting "Run as administrator".
2. Ensure Docker desktop is running.
3. Run the supplied `atmos-launcher.exe` to start the installation process.
4. Follow the prompts to complete the installation.
5. After installation, you can start the Atmos Launcher by running `atmos.exe` from the installation directory.

For a default installation the path will be at `C:\Program Files\Atmos\app\atmos.exe`.

When launching the application, a terminal will appear. These are the logs for the application. Closing the terminal will close the application.
Within those logs you will see the following within the first few lines:

![img_1.png](assets/localhost.png)

This is the port that the application is running on. You can access the web interface by navigating to `http://localhost:PORT` in your web browser, where `PORT` is the port number shown in the logs.
If you want to connect from another device on the same network, replace `localhost` with the IP address of the machine running Atmos.

**Potential Errors:**

**Migration failed. Are you sure the database is running?**

If you encounter the previous error message, ensure that docker is running. If it already was try running the installer again. 

Output should look like this on the first run:

![img.png](assets/migration-error.png)

Subsequent successful runs should look like this:

![img_1.png](assets/success.png)