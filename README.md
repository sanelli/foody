# Foody

Small C# console app that reads a food-intake JSON file and logs it to the
[Google Health API](https://developers.google.com/health) (`nutrition-log` data type).

## Requirements

- .NET SDK matching `global.json` (10.0.x, roll-forward latest feature)

## Build

```bash
dotnet build Foody.slnx
```

Warnings are treated as errors. Analyzers enabled:

- StyleCop.Analyzers.Unstable
- SonarAnalyzer.CSharp
- Comment.Todo.Analyzer
- .NET analyzers at `AnalysisLevel=latest-all` / `AnalysisMode=All`

## Run

```bash
dotnet run --project Foody.csproj -- intake Samples/sample-intake.json
```

Or after publish / build, the assembly name is `foody`:

```bash
foody intake path-to-the-json-file
foody sample
foody schema
foody list mealtype
foody help
```

## Sample intake JSON

See `Samples/sample-intake.json`, or generate one with `foody sample`:

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
| Endpoint | `POST https://health.googleapis.com/v4/users/me/dataTypes/nutrition-log/dataPoints` |
| Scope | `https://www.googleapis.com/auth/googlehealth.nutrition.writeonly` |

### Credentials (not configured yet)

Copy `.env.example` and set:

| Variable | Purpose |
|----------|---------|
| `FOODY_GOOGLE_CLIENT_ID` | OAuth 2.0 Client ID (Google Cloud Console) |
| `FOODY_GOOGLE_CLIENT_SECRET` | OAuth 2.0 Client Secret |
| `FOODY_GOOGLE_ACCESS_TOKEN` | Short-lived Bearer token for API calls |
| `FOODY_GOOGLE_REFRESH_TOKEN` | Offline refresh token (`access_type=offline`) |

Enable **Google Health API** in Google Cloud, create an OAuth client, and allow the nutrition write-only scope on the Data Access page. Auth uses the user OAuth authorization-code flow (not service accounts alone).

## License

MIT — see `LICENSE`.
