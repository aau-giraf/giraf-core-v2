## Guide

Run
`docker compose up --build`

Then check the following endpoints:
`localhost:8080/health`
`localhost:8080/colors`

The `/colors` endpoint should return an empty array. To add entries to the colors table, run:
`docker exec -it giraf-core-v2-db-1 psql -U postgres -d defaultdb`

Now you can run `INSERT INTO "Colors" ("Id", "ColorName") VALUES (1, 'Red');`

## Project Structure (outdated)

```text
giraf-core-v2/
├── Data/                  # Database context
│   └── Migrations/        # EF Core migrations
├── Endpoints/             # API endpoints
├── Entities/
│   └── DTOs/
├── Mappings/              # Mapping between entities and DTOs
│   ├── ToDTO/
│   └── ToEntity/
├── Properties/
│   └── launchSettings.json
├── Services/              # Business logic
├── Utilities/
├── appsettings.json
├── appsettings.Development.json
├── core.csproj
├── core.http
├── Program.cs
└── LICENSE
```
