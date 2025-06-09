# Use Cases

- Which unit should be the source of truth in the database?
- Do we need to support multiple units?
- What happens when the program starts? Does it read the last known values from a persistent store or start fresh?
- How much historical data should be kept? Is there a retention policy?

# Technical Questions

- How should the application handle the case where the RS-485 sensor is not available or fails to respond?
- How should migrations be handled? 


**Additional Questions to Consider (Self-Hosted Docker Focus):**

**I. Configuration & Deployment (Docker Context):**

1.  **Environment Configuration:** How will users configure application-specific settings (e.g., RS-485 serial port path, sensor polling interval, aggregation window for hydration, database connection string if not default Docker networking)?
    *   _Consider:_ Environment variables in `docker-compose.yml`, a mounted `.env` file, or a mounted configuration file for the backend.
2.  **Database Persistence:** How will the PostgreSQL data be persisted across container restarts/recreations?
    *   _Consider:_ Docker named volumes (most common and recommended).
3.  **Network Configuration:** How will the containers communicate? (e.g., backend to Postgres, frontend to backend).
    *   _Consider:_ Docker Compose default networking is usually sufficient, but worth confirming service names for connection strings.
4.  **Logging Strategy:** How should logging be handled for each container (backend, Postgres, frontend)?
    *   _Consider:_ Directing logs to `stdout/stderr` so `docker logs <container_name>` works. Should logs also go to a persistent volume for longer-term storage/troubleshooting?
5.  **Application Updates/Upgrades:** How will the user update the application (backend image, frontend image, potentially Postgres image)?
    *   _Consider:_ `docker-compose pull && docker-compose up -d`. How will database migrations be triggered during an update? (Often a script or entrypoint command in the backend container).
6.  **Initial Setup/Seeding:** Is any initial data seeding required beyond migrations (e.g., default user, default configuration if not using env vars)?

**II. Operational & Resilience (Self-Hosted Context):**

8.  **Backup and Restore:** What's the recommended procedure for backing up and restoring the sensor data (PostgreSQL database)?
    *   _Consider:_ `pg_dump` and `pg_restore`. Should there be a helper script or documentation for this?
9.  **Error Indication & Troubleshooting:** Beyond logs, how will the user know if something is wrong (e.g., sensor permanently disconnected, database down)?
    *   _Consider:_ A status indicator in the UI? Email alerts (might be overkill for a simple self-hosted app, but possible)?
10. **Handling "Bad" Sensor Data:** What should happen if the sensor returns clearly erroneous or out-of-bounds data (e.g., temp of -273.16 C, humidity > 100%)?
    *   _Consider:_ Log it, discard it, store it but flag it? This impacts averages and extremes.
12. **Data Retention Policy:** How long should sensor readings be stored in the database by default? Is there a mechanism or recommendation for pruning old data to manage disk space?

**III. User Experience & Features (Beyond Core Polling):**

13. **Historical Data Access:** How does the user expect to view/query historical data beyond the real-time dashboard?
    *   _Consider:_ Date range pickers, simple charts, ability to export a range to CSV?
14. **Manual Data Adjustments/Deletions:** Is there ever a scenario where a user might need to manually correct or delete a specific reading or a range of readings? (Generally avoided, but good to ask).
15. **Security Considerations (Even for Self-Hosted):**
    *   How will the Postgres database be secured (e.g., default password, user access)? (Docker secrets can manage this).
    *   Will the web interface be exposed to the local network only, or potentially to the internet? If the latter, HTTPS is a must.
    *   Is any form of authentication needed for the web UI, even for a single user (e.g., to access admin functions like restart/dehydrate)?

**I. Understanding Their Needs & Expectations (Data Usage & Display):**

1.  **The "Why":** "What's the main reason you want to track temperature, humidity, and dew point? What problem are you trying to solve or what are you hoping to learn?"
    *   _Impact:_ Helps prioritize what data is most important, how it should be presented, and what "insights" they're looking for.
2.  **Looking Back - Data History:**
    *   "How far back in time do you imagine needing to look at your sensor readings? A week? A month? A year? Forever?" (This is your data retention question, phrased for the user).
    *   "When you look at older data (say, from last month), what kind of information are you usually trying to find?" (e.g., "What was the hottest day?" "Was it always this humid in July?")
    *   "Is older data (say, from a year ago) just as important in full detail as data from yesterday, or would a summary (like daily averages) be okay for very old data?"
    *   _Impact:_ Directly influences database size, data retention policies, potential for data aggregation/archiving, and how historical data is queried and displayed.
3.  **Real-Time View:**
    *   "On the main screen, what's the *most* important piece of information for you to see at a glance?" (e.g., current temp, 1-minute trend, daily high/low).
    *   "You mentioned daily minimums and maximums. When a new day starts (at midnight), do you want these to reset automatically for the new day?"
    *   _Impact:_ Confirms the `UpdateDto` contents and the logic for daily extreme resets.
4.  **Dealing with Gaps or Issues:**
    *   "If the sensor stops sending data for a while (maybe it gets unplugged or there's a power blip), what would you expect to see on the screen when it comes back online? Should it try to show you what happened while it was off, or just pick up with new readings?" (Relates to hydration and how gaps are displayed).
    *   "If the sensor readings suddenly look very strange or impossible (like a temperature of -100 degrees), what do you think the app should do? Should it ignore it, show it with a warning, or something else?"
    *   _Impact:_ Error handling for sensor data, display of missing data, data validation rules.
5.  **Units & Display Preferences:**
    *   "What units do you prefer for temperature? Celsius or Fahrenheit?" (You have this, it's key).
    *   "Are there any other ways you'd like to see the data displayed that might be helpful (e.g., simple charts showing trends over the last hour or day)?"
    *   _Impact:_ UI design, conversion logic, potential for basic charting features.

**II. Using the Application & "What Ifs":**

6.  **Starting Up:** "When you first start the application (or open the webpage), what do you expect to see immediately? Should it show the very latest readings, or perhaps a summary of the last few minutes?"
    *   _Impact:_ Reinforces decisions about initial data load and hydration.
7.  **Making Changes (User-Triggered Actions):**
    *   "If you needed to change a setting, or if things seemed a bit stuck, would you want an easy way to 'refresh' or 'restart' the part that's gathering sensor data, without losing all your old history?" (Explains `React triggered restart` in user terms).
    *   "Similarly, would you ever want to clear out the recent calculations (like the 1-minute averages) and have them start fresh, perhaps if you moved the sensor?" (Explains `React triggered dehydrate`).
    *   _Impact:_ Confirms the need and user understanding of these admin-like functions.
8.  **Data Export:** "Might you ever need to get your sensor data out of this application, perhaps to look at in a spreadsheet or for other records?"
    *   _Impact:_ Potential need for a data export feature (e.g., CSV export from the Web API).

**III. Long-Term & Big Picture:**

9.  **Growth/Changes:** "Do you imagine ever wanting to monitor more than one sensor location with this system in the future?" (Even if the answer is no now, it's good to gauge).
    *   _Impact:_ Scalability considerations, database schema if multiple sensors become a possibility (though your current plan is single).
10. **Alerts (Future-Proofing):** "Are there certain temperature or humidity levels that you'd consider 'too high' or 'too low' that you'd want to be easily made aware of, even if you're not looking at the screen?" (This hints at future alerting features).
    *   _Impact:_ Not for immediate data flow, but good for long-term vision.

**Key is to phrase these in terms of *their experience* and *what they want to achieve*.** Avoid technical terms like "database," "API," "cache" unless they bring it up or you can explain it very simply in context.

For example, instead of "How should the aggregator hydrate its cache?", you ask "When you first open the app... what do you expect to see?"

This approach will give you valuable insights into how the user perceives and intends to use the application, which will directly inform your implementation decisions, including the data flow.