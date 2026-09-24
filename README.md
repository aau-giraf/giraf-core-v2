## Project Structure

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
