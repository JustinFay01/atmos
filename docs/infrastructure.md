# Infrastructure Layer 

## Running Migrations

In the root of the project directory, run the following command to apply migrations:

```bash
dotnet ef database update -s .\Atmos\ -p .\Atmos\Infrastructure\
```

To add a new migration, use:

```bash
dotnet ef migrations add <MigrationName> -s .\Atmos\ -p .\Atmos\Infrastructure\
```