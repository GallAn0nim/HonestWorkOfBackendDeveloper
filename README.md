# Honest work of a backend developer

A plain personal page (name, three links) with a magnifier. The lens reveals a honeycomb of small
backend experiments that really run: a **Sorting Engine** (Bubble, Merge, Purge Sort) and a
**Maze** hex that first builds a maze (randomized Prim) and then solves it (DFS, BFS, A*, Dijkstra,
Greedy Best-First, Bidirectional BFS, Trémaux, Drunk Mouse, Wall Hugger).

The .NET API hands out each run's data and solves it; the page plays the steps back and checks the
result itself. When the API can't be reached (for example on a static host), the run says so.

## Requirements

- Node.js 20.19+ (or 22.12+) and npm, as Vite 8 requires
- .NET 10 SDK (for the API and its tests)
- Google Chrome (the browser tests use the installed Chrome; nothing is downloaded)

## Run locally

```bash
npm install
npm run api   # .NET API on http://localhost:5280
npm run dev   # page on http://localhost:5173, /api is proxied to the API
```

`npm run dev` alone also works; runs then report the API as unreachable.

## Test

```bash
npm test                  # unit tests (Vitest): validation, API data checks
cd api && dotnet test     # API tests (xUnit): algorithms, generators, endpoints
npm run test:e2e          # browser tests (Playwright): page checklist, API, offline
```

`npm run test:e2e` starts its own Vite server and the API (or reuses a running `npm run api`).

## Layout

| Path | What |
|---|---|
| `index.html`, `src/style.css` | the page and its styles |
| `src/main.ts` | honeycomb, lens, open-hex dialog; the slot list of modules |
| `src/engine.ts` | shared module shell: playback, budget, API call |
| `src/sorting.ts`, `src/maze.ts` | algorithm names, validation, API data checks (no DOM, no network) |
| `src/sortingModule.ts`, `src/mazeModule.ts` | the sorting and maze modules |
| `api/HonestWork.Api` | .NET 10 minimal API: `GET /api/sort/random`, `POST /api/sort`, `GET /api/maze/generation`, `POST /api/maze` |
| `src/observabilityModule.ts` | Observability hex: live logs (SSE from `GET /api/observability/logs`) and metrics (`GET /api/observability/metrics`) |
| `e2e/prd.e2e.ts` | browser tests |

## Deploy

The page builds to static files (`npm run build` → `dist/`). To use the API in production, serve it
on the same domain under `/api` (for example behind a reverse proxy); a different domain would also
need CORS. Without the API, the page still loads but runs report the API as unreachable.

The API reads its database from `ConnectionStrings:HonestWork`, which is not in `appsettings.json`.
Locally it comes from user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:HonestWork" "Host=...;Database=...;Username=...;Password=..." --project api/HonestWork.Api
```

In production set the `ConnectionStrings__HonestWork` environment variable. Outside Development the API
redirects HTTP to HTTPS, sends HSTS and marks the session cookie `Secure`. The proxy terminates TLS and
must send `X-Forwarded-For` and `X-Forwarded-Proto`; it is trusted only when it runs on the same host.
Rate limits per client IP live in the `RateLimiting` section of `appsettings.json`.
