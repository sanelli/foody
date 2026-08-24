# Foody

C# tooling to log food intake to the
[Google Health API](https://developers.google.com/health) (`nutrition-log` data type).

## Projects

| Project | Role |
|---------|------|
| `Foody.Lib` | String-oriented core (no file/console I/O) |
| `Foody` | Console CLI (`foody`) |
| `Foody.Mcp` | MCP server exposing the same operations over stdio |

Shared settings: root `.editorconfig`, `Directory.Build.props` (warnings as errors, analyzers), `stylecop.json`.

## Requirements

- .NET SDK matching `global.json` (10.0.x, roll-forward latest feature)

## Build

```bash
dotnet build Foody.slnx
```

Analyzers (all projects): StyleCop.Analyzers.Unstable, SonarAnalyzer.CSharp, Comment.Todo.Analyzer, `AnalysisLevel=latest-all` / `AnalysisMode=All`.

## CLI

```bash
dotnet run --project Foody/Foody.csproj -- intake Foody/Samples/sample-intake.json
dotnet run --project Foody/Foody.csproj -- bulk-intake Foody/Samples/sample-bulk-intake.json
dotnet run --project Foody/Foody.csproj -- sample
dotnet run --project Foody/Foody.csproj -- schema intake
dotnet run --project Foody/Foody.csproj -- schema bulk-intake
dotnet run --project Foody/Foody.csproj -- list mealtype
dotnet run --project Foody/Foody.csproj -- list schema
dotnet run --project Foody/Foody.csproj -- help
```

After publish, the assembly name is `foody`.

## MCP server

```bash
dotnet run --project Foody.Mcp/Foody.Mcp.csproj
```

Tools: `intake`, `bulk_intake`, `sample`, `schema`, `list_mealtype`, `list_schema`, `help`.

Example Cursor MCP config:

```json
{
  "mcpServers": {
    "foody": {
      "command": "dotnet",
      "args": ["run", "--project", "/absolute/path/to/foody/Foody.Mcp/Foody.Mcp.csproj"]
    }
  }
}
```

## Sample intake JSON

```bash
foody sample
```

```json
{
  "foodDisplayName": "Banana",
  "mealType": "SNACK",
  "startTime": "2026-08-24T15:00:00Z",
  "endTime": "2026-08-24T15:05:00Z",
  "energyKcal": 105,
  "totalCarbohydrateGrams": 27,
  "totalFatGrams": 0.3,
  "totalProteinGrams": 1.3
}
```

## Google Health API

| Item | Value |
|------|--------|
| API | Google Health API v4 |
| Endpoint (single) | `POST .../nutrition-log/dataPoints` |
| Endpoint (bulk) | `POST .../nutrition-log/dataPoints:import` (max 10000) |
| Scope | `https://www.googleapis.com/auth/googlehealth.nutrition.writeonly` |

### Credentials (not configured yet)

Copy `.env.example` and set:

| Variable | Purpose |
|----------|---------|
| `FOODY_GOOGLE_CLIENT_ID` | OAuth 2.0 Client ID (Google Cloud Console) |
| `FOODY_GOOGLE_CLIENT_SECRET` | OAuth 2.0 Client Secret |
| `FOODY_GOOGLE_ACCESS_TOKEN` | Short-lived Bearer token for API calls |
| `FOODY_GOOGLE_REFRESH_TOKEN` | Offline refresh token (`access_type=offline`) |

Enable **Google Health API** in Google Cloud, create an OAuth client, and allow the nutrition write-only scope on the Data Access page.

## License

MIT — see `LICENSE`.
