# GitHub Copilot Instructions

## Project Overview

HMPPS CFO Data Management System (DMS) — a .NET 10 distributed microservices application that processes p-NOMIS (Offloc) and nDelius offender data to supply CATS (Case Assessment and Tracking System) with accurate offender records. Targets Windows Server EC2 (deployed as Windows Services) and runs locally via .NET Aspire.

## Build, Test & Run

```bash
# Build the solution
dotnet build

# Run all tests
dotnet test --configuration Release

# Run tests for a specific project
dotnet test tests/Matching.Engine.Tests/Matching.Engine.Tests.csproj --configuration Release

# Run a single test
dotnet test tests/Api.Tests/Api.Tests.csproj --filter "FullyQualifiedName~SearchAsync_WithNoMatches_ReturnsNotFound"

# Run locally via Aspire (recommended — starts all services + dependencies)
# VS Code: F5 → Default Configuration
# Visual Studio: select "Aspire.AppHost" debug config

# Deploy databases to a test environment
export SERVER="..." DB_USER="..." DB_PASS="..."
python3 publish_db.py          # deploy
python3 publish_db.py --dry-run  # preview only

# Seed test data
dotnet run --project ./src/FakeDataSeeder/FakeDataSeeder.csproj
```

## Architecture

### Data Pipeline

```
FileSync → [Offloc.Cleaner → Offloc.Parser | Delius.Parser] → Import → DbInteractions → Blocking → Matching.Engine → API / Visualiser
```

All inter-service communication is **asynchronous via RabbitMQ** using the [Rebus](https://github.com/rebus-org/Rebus) library. Each stage publishes a `*FinishedMessage` that triggers the next stage.

### Services

| Service | Type | Role |
|---------|------|------|
| `FileSync` | Worker | Monitors MinIO/S3/filesystem for incoming files |
| `Offloc.Cleaner` | Worker | Cleans raw Offloc (p-NOMIS) files |
| `Offloc.Parser` | Worker | Parses cleaned Offloc files into DB records |
| `Delius.Parser` | Worker | Parses nDelius files into DB records |
| `Import` | Worker | Coordinates staging → running picture migration |
| `DbInteractions` | Worker | Executes DB staging/merge operations (runs in SQL container) |
| `Blocking` | Worker | Generates candidate record pairs for matching |
| `Matching.Engine` | Worker | Compares pairs (Comparator), scores (Scorer, Bayesian), clusters |
| `Cleanup` | Worker | Data maintenance |
| `Logging` | Worker | Centralised log aggregation |
| `Meow` | Worker | CATS RabbitMQ integration (different broker config) |
| `API` | ASP.NET Core | REST endpoints for downstream consumers |
| `Visualiser` | ASP.NET Core | Blazor web UI for exploring offender relationships |

### Databases (SQL Server)

Seven separate databases: `OfflocStagingDb`, `OfflocRunningPictureDb`, `DeliusStagingDb`, `DeliusRunningPictureDb`, `MatchingDb`, `ClusterDb`, `AuditDb`. Database schemas are managed as SQL Database Projects under `src/Database/`.

### Shared Libraries (`src/Libraries/`)

- **`Messaging`** — RabbitMQ integration via Rebus; all message types; `Exchanges` constants; `AddDmsRabbitMQ()` extension
- **`Infrastructure`** — EF Core `DbContext`s (`OfflocContext`, `DeliusContext`, `ClusteringContext`, `AuditContext`), entity models, repositories, shared DTOs
- **`Matching.Core`** — `IMatcher<T, Result>` interface and concrete matchers (Jaro-Winkler, Levenshtein, Caver, Date, Postcode, Equality); `[Matcher("key")]` attribute for dynamic discovery
- **`EnvironmentSetup`** — `AddDmsCoreWorkerService()` and `UseDmsSerilog()` extension methods shared by all worker services; `FileLocations` / `FilePatterns`

## Key Conventions

### Service Bootstrap Pattern

All worker services follow the same bootstrap pattern in `Program.cs`:

```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.AddDmsCoreWorkerService();          // Serilog + Windows Service + file locations
builder.Services.AddDmsRabbitMQ(builder.Configuration);
// ... register additional services
var app = builder.Build();
await app.RunAsync();
```

`Meow` and `API`/`Visualiser` are exceptions — they configure messaging or hosting differently.

### Messaging

- All messages implement `IMessage` from `Messaging.Messages`
- Messages are grouped by pipeline stage: `BlockingMessages`, `DbMessages`, `ImportMessages`, `MatchingMessages`, `StagingMessages`, `MergingMessages`, `StatusMessages`
- Exchange names are string constants in `Messaging.Exchanges` (lowercase: `staging`, `merging`, `database`, etc.)
- RabbitMQ connection string is pulled from `ConnectionStrings:RabbitMQ` in config

### Dependency Injection

- Worker services use standard `Microsoft.Extensions.DependencyInjection`
- `Matching.Engine` additionally uses **Autofac** (via `AutofacServiceProviderFactory`) for registering matchers and scorers dynamically via reflection

### Matching Engine

- `[Matcher("key")]` attribute decorates matcher classes for dynamic registration
- Three hosted services run in parallel within one process: `ComparatorService`, `ScorerService`, `ClusteringService`
- `MatchingQueue` is a singleton in-memory queue between comparator and scorer
- Scoring uses Bayesian probability; matchers include string similarity algorithms (Jaro-Winkler, Levenshtein) and phonetic matching (Caver/Soundex)

### API Authentication

The API supports two auth schemes via a `"Smart"` policy scheme:
- **JWT Bearer** (Entra ID / Microsoft Identity) — used for `dms.read`, `dms.write`, `visualiser.read`, `visualiser.write` scopes
- **Legacy API Key** (`X-API-KEY` header) — for backward compatibility

Swagger UI is only enabled when `IsDevelopment=true` (passed via Aspire parameter).

### Package Management

All NuGet package versions are centrally managed in `Directory.Packages.props` — never specify versions in individual `.csproj` files.

### Test Patterns

- Framework: **xunit** with `EF Core InMemory` for repository/endpoint tests
- Tests use `IDisposable` to call `context.Database.EnsureDeleted()` in teardown
- Each test creates a database with a unique name (`$"TestDb_{Guid.NewGuid()}"`) to avoid cross-test contamination
- Integration tests for messaging use **Testcontainers** (RabbitMQ)
- Arrange/Act/Assert comment blocks are used consistently

### Aspire Configuration

`Parameters:startCoreServices` (bool) controls whether RabbitMQ, MinIO, and all worker services are started — set to `false` to run API + Visualiser only. `Parameters:seedData` triggers `FakeDataSeeder` on startup.
