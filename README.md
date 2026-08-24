# Foody

C# tooling to log food intake to the
[Google Health API](https://developers.google.com/health) (`nutrition-log` data type).

## Projects

| Project | Role |
|---------|------|
| `Foody.Lib` | String-oriented core (no file/console I/O) |
| `Foody` | Console CLI (`foody`) |
| `Foody.Mcp` | MCP server exposing the same operations over stdio |
| `Foody.Gemini` | Converts a markdown food diary (+ images) via Gemini into bulk intake (`foody-gemini`) |

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

## Gemini diary importer

```bash
dotnet run --project Foody.Gemini/Foody.Gemini.csproj -- path/to/diary.md
```

`foody-gemini` reads the markdown file, resolves image references in the same folder, compresses each image to under 250 KB, asks Gemini (Google AI Studio API key) for Foody `bulk-intake` JSON only, extracts the JSON array from the response, then uploads it with `Foody.Lib`.

Configure `Foody.Gemini/appsettings.json` (and optional `appsettings.Local.json`):

| Setting | Purpose |
|---------|---------|
| `Gemini:ApiKey` | API key from [Google AI Studio](https://aistudio.google.com/) |
| `Gemini:Model` | Gemini model id (default `gemini-2.5-flash`) |
| `GoogleHealth:*` | Same Google Health credentials as the other hosts |

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

- Docs: [Google Health API](https://developers.google.com/health)
- Codelab: [Make your first API call](https://developers.google.com/health/codelabs/make-your-first-api-call#1)

| Item | Value |
|------|--------|
| API | Google Health API v4 |
| Endpoint (single) | `POST .../nutrition-log/dataPoints` |
| Endpoint (bulk) | `POST .../nutrition-log/dataPoints:import` (max 10000) |
| Scope | `https://www.googleapis.com/auth/googlehealth.nutrition.writeonly` |

### Credentials (not configured yet)

Each host has a checked-in `appsettings.json`:

- `Foody/appsettings.json`
- `Foody.Mcp/appsettings.json`
- `Foody.Gemini/appsettings.json`

Optional local overrides (gitignored): `appsettings.Local.json` next to the host `appsettings.json`. Values in Local override the base file.

| Setting | Purpose |
|---------|---------|
| `GoogleHealth:ClientId` | OAuth 2.0 Client ID (Google Cloud Console) |
| `GoogleHealth:ClientSecret` | OAuth 2.0 Client Secret |
| `GoogleHealth:AccessToken` | Short-lived Bearer token for API calls |
| `GoogleHealth:RefreshToken` | Offline refresh token (`access_type=offline`) |

Example `appsettings.Local.json`:

```json
{
  "GoogleHealth": {
    "ClientId": "...",
    "ClientSecret": "...",
    "AccessToken": "...",
    "RefreshToken": "..."
  }
}
```

Enable **Google Health API** in Google Cloud, create an OAuth client, and allow the nutrition write-only scope on the Data Access page.

## License

MIT — see `LICENSE`.
