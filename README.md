# Honest work of a backend developer

**A portfolio you can't fake.** Backend work is invisible: no one sees a race condition you
prevented or a query you cached. This page makes it visible.

At first glance it is a plain personal page: a name and three links. Pick up the magnifier and the
lens reveals a honeycomb of live backend experiments. Each hex is a real .NET 11 API talking to a
real PostgreSQL database. Nothing is mocked, nothing is a pre-recorded animation.

## What's under the lens

**Sorting Engine.** Eleven algorithms, from Quick, Merge and Heap Sort to Bogo, Bozo and Purge Sort
("extremely fast at the minor expense of deleting data"). The API sorts; the page replays every step
and checks the result itself. When Purge Sort "wins", the contract check says so.

**Maze.** The API builds a maze with randomized Prim, then solves it with A\*, Dijkstra, BFS, DFS,
Greedy Best-First, Bidirectional BFS, Trémaux, a Wall Hugger and a Drunk Mouse. Or race five
solvers against each other and bet on the winner.

**Cache.** Every request runs a real sales report in PostgreSQL. Send more than 4 requests a second
and the API switches to .NET HybridCache on the fly. Watch latency drop and DB queries stop.

**Race condition.** 20 customers try to buy the last copy of a comic at the same instant. Run it
three ways:

- **Unsafe**: read, then write. The same copy sells many times.
- **Atomic**: one `UPDATE … WHERE "Stock" > 0`. Exactly one sale.
- **Queue**: a channel with a single consumer. Exactly one sale, every request traced.

**Login.** A real auth flow: ASP.NET Core Identity hashes, constant-time checks against user
enumeration, HttpOnly SameSite=Strict session cookie, and password recovery. Signing in unlocks the
other hexes.

**Observability.** A live look inside the API serving the page: logs streamed over Server-Sent
Events, p50/p95 latency per route from ASP.NET Core's own metrics, and distributed traces of queued
races carried across the channel. No Prometheus, no Grafana: built from `ILogger`, `MeterListener`
and `ActivityListener`.

**(In)Sanity Check.** A random developer quote from PostgreSQL. A short break between the serious
hexes.

**Curriculum Vitae.** Generated as a PDF on request from the database with QuestPDF.

## How it's built

- **All algorithms run on the backend.** The C# API generates, solves and records every step. The
  TypeScript front end only replays and validates, so the page proves the API's answer is right.
- **Vanilla TypeScript and Vite.** No framework. The page is three links and one widget.
- **Production habits in a side project.** Per-IP rate limits, HTTPS and HSTS outside Development,
  forwarded-header handling behind a proxy, EF Core migrations and Docker Compose.
- **Tested at every layer.** xUnit for algorithms and endpoints, Vitest for validation, Playwright
  for the page, the API and offline behavior.
- **Accessible.** Keyboard reachable, 12 px minimum text at 4.5:1 contrast, reduced-motion support,
  and a lens that floats above your finger on touch screens.

## Try it

```bash
docker compose up --build -d   # then open http://localhost:8080
```

Local dev without Docker: `npm install`, `npm run api` and `npm run dev`. Tests: `npm test`,
`cd api && dotnet test` and `npm run test:e2e`.

## Educational purpose: what am I seeing?

Quick facts behind each hex, in plain words.

### Sorting Engine

The API sorts 12 random values and records every compare and swap. The page replays them as bars.

- **Bubble Sort** swaps neighbours until nothing moves. O(n²).
- **Selection Sort** finds the smallest value and puts it next in line. O(n²).
- **Insertion Sort** slides each value back into the sorted part, like sorting cards in your hand. O(n²), fast on almost-sorted data.
- **Quick Sort** picks a pivot, puts smaller values left and bigger right, then repeats on each side. O(n log n) on average.
- **Merge Sort** splits the array in half until single values, then merges sorted halves. Always O(n log n).
- **Heap Sort** builds a heap (a tree where the parent is the biggest), then keeps moving the top to the end. O(n log n), no extra memory.
- **Shell Sort** is Insertion Sort with big jumps first, then smaller ones.
- **Cycle Sort** writes each value straight to its final place. Fewest writes possible.
- **Bogo Sort** shuffles until sorted. **Bozo Sort** swaps two random values until sorted. Both are jokes, with factorial time.
- **Purge Sort** deletes every value smaller than the last one kept. O(n) and "sorted", but data is gone. That is why the page checks the result: sorted is not enough, nothing may be lost.

### Maze

- **Generation: randomized Prim.** Start in one cell. Keep a list of walls at the edge of the visited area. Knock down a random one if it leads to a new cell. The result is a perfect maze: exactly one path between any two cells. Then 15% extra walls are removed, so loops exist and solvers really differ.
- **Entrance and exit** are placed as far apart as possible.
- **BFS** explores in rings, layer by layer. Finds the shortest path.
- **DFS** goes as deep as it can, then backs up. Finds a path, often not the shortest.
- **Dijkstra** expands the cheapest cell first. On a grid where each step costs 1, it acts like BFS.
- **A\*** is Dijkstra plus a guess of the distance left (a heuristic). Shortest path, fewer cells explored.
- **Greedy Best-First** follows only the guess. Fast, but can be fooled into long detours.
- **Bidirectional BFS** searches from both ends and stops where they meet.
- **Trémaux** marks passages as it walks and never takes one more than twice. A 19th-century method for real mazes.
- **Wall Hugger** keeps its right hand on the wall. It always gets out here, because both doors are on the outer wall, but it often takes the long way.
- **Drunk Mouse** picks a random direction every step. It gets there, eventually.
- **Race** runs five random solvers on the same maze at once. Pick the winner before it starts.

### Cache

- Each request runs a heavy sales report in PostgreSQL (orders joined with products and categories, grouped and summed).
- Above 4 requests a second, the API serves the report from memory with .NET HybridCache for 30 seconds. Only a cache miss touches the database.
- Watch latency fall and the DB query counter stop. This is the cache-aside pattern: check the cache, on a miss ask the database and store the answer.
- At most 4 queries hit the database at once, so a burst can't overload it.

### Race condition

The problem: 20 people buy the last copy at the same moment.

- **Unsafe** reads the stock, then writes. All 20 see "1 left", so all 20 buy. This is a lost update (check-then-act).
- **Atomic** checks and buys in one SQL statement (`UPDATE … WHERE "Stock" > 0`). The database lets only one through.
- **Queue** puts every request in line and one worker handles them one by one. No race is possible, at the cost of waiting.

### Login

- Passwords are stored as salted hashes (ASP.NET Core Identity), never as plain text.
- An unknown username is still checked against a dummy hash. A wrong user and a wrong password take the same time, so timing doesn't reveal which usernames exist.
- The session is an HttpOnly, SameSite=Strict cookie. JavaScript can't read it, and other sites can't send it.
- Recovery suggests the closest usernames by Levenshtein distance: the fewest letter edits that turn one word into another.

### Observability

The three pillars of observability, built by hand:

- **Logs** stream live over Server-Sent Events: one HTTP connection that the server keeps writing to.
- **Metrics** show requests per second and p50/p95 latency per route. p95 means 95% of requests were faster than this.
- **Traces** follow one queued race through its steps. The trace ID travels with the message through the queue, so the worker's work links back to the request that started it.

### (In)Sanity Check

A random quote picked from PostgreSQL, never the same twice in a row.

### Curriculum Vitae

`GET /api/cv` builds the PDF on every request from database rows with QuestPDF. Change the data, get a new CV. No file is stored.

## License

MIT. See [LICENSE](LICENSE).
