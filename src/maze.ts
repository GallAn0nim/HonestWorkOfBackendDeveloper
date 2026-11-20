export type Maze = { width: number; height: number; cells: number[]; entrance: number; exit: number; seed: number };
export type MazeStep = { type: 'visit'; index: number } | { type: 'path'; index: number };
export type GenerationStep = { type: 'carve' | 'open' | 'door'; cell: number; side: number };
export type Solver = { key: string; name: string; big: string; info: string };

export const SIZE = 30;
const NORTH = 1;
const EAST = 2;
const SOUTH = 4;
const WEST = 8;
const ALL_WALLS = NORTH | EAST | SOUTH | WEST;
const OUTSIDE = -1;
const OPPOSITE: Record<number, number> = { [NORTH]: SOUTH, [EAST]: WEST, [SOUTH]: NORTH, [WEST]: EAST };

export function fromApi(body: unknown): Maze {
  const maze = body as Maze;
  const isCount = (value: unknown) => Number.isInteger(value) && (value as number) > 0;
  const isCell = (value: unknown) => Number.isInteger(value) && (value as number) >= 0 && (value as number) < maze.cells.length;
  const wellFormed =
    isCount(maze?.width) &&
    isCount(maze.height) &&
    Number.isInteger(maze.seed) &&
    Array.isArray(maze.cells) &&
    maze.cells.length === maze.width * maze.height &&
    maze.cells.every((walls) => Number.isInteger(walls) && walls >= 0 && walls <= ALL_WALLS) &&
    isCell(maze.entrance) &&
    isCell(maze.exit) &&
    maze.entrance !== maze.exit;

  if (!wellFormed) {
    throw new Error(`Bad maze from API: ${JSON.stringify(body)}`);
  }

  return maze;
}

export function wallPath(maze: Maze) {
  return maze.cells
    .map((walls, cell) => {
      const x = cell % maze.width;
      const y = Math.floor(cell / maze.width);
      const north = walls & NORTH ? `M${x} ${y}h1` : '';
      const west = walls & WEST ? `M${x} ${y}v1` : '';
      const south = walls & SOUTH && y === maze.height - 1 ? `M${x} ${y + 1}h1` : '';
      const east = walls & EAST && x === maze.width - 1 ? `M${x + 1} ${y}v1` : '';

      return north + west + south + east;
    })
    .join('');
}

export const SOLVERS: Solver[] = [
  { key: 'dfs', name: 'Depth-First Search', big: 'stack', info: 'Runs down one corridor until it hits a wall, then backs up. It keeps its route on a stack: each step pushes an unvisited neighbour, a dead end pops back to the last fork. It finds a way out, not necessarily the shortest one.' },
  { key: 'bfs', name: 'Breadth-First Search', big: 'queue', info: 'Spreads out evenly like water. A queue visits every cell one step from the entrance, then every cell two steps away, and so on, so the first route to reach the exit is always the shortest.' },
  { key: 'astar', name: 'A* Search', big: 'f = g + h', info: 'Knows roughly where the exit is. It always expands the open cell with the lowest f = g + h, where g is the steps walked so far and h is the Manhattan distance to the exit, ignoring walls. It still finds the shortest route, while exploring fewer cells than Dijkstra.' },
  { key: 'dijkstra', name: 'Dijkstra', big: 'f = g', info: 'Checks everything nearby first. It always expands the open cell with the fewest steps walked from the entrance (f = g), so it finds the shortest route without any idea where the exit is.' },
  { key: 'greedy', name: 'Greedy Best-First', big: 'f = h', info: 'Heads straight for the exit and ignores how far it has walked. It always expands the open cell closest to the exit (f = h). Often fast, but the route it finds can be far from the shortest.' },
  { key: 'bidirectional', name: 'Bidirectional BFS', big: '2 queues', info: 'Searches from both ends at once and meets in the middle. Two breadth-first queues take turns, one from the entrance and one from the exit, until a cell is reached by both. Each side only covers about half the distance.' },
  { key: 'tremaux', name: 'Trémaux', big: 'chalk marks', info: 'Marks every passage it walks and never takes a marked one twice. At a dead end it walks back to the last cell with an unmarked way out. It needs no map and always finds the exit.' },
  { key: 'drunkmouse', name: 'Drunk Mouse', big: 'random walk', info: 'Stumbles in a random direction every step. Gets there, eventually. Maybe. When it steps onto a cell it already walked through, the loop is cut from its route. It gives up after a move limit.' },
  { key: 'wallhugger', name: 'Wall Hugger', big: 'no map', info: 'One hand on the wall. At every cell it turns right if it can, else goes straight, else left, else turns back. It needs no map, but loops in the maze can make it walk the same corridors again. It gives up after a move limit.' },
];

function wallBetween(maze: Maze, from: number, to: number) {
  if (to === from - maze.width) {
    return NORTH;
  }

  if (to === from + maze.width) {
    return SOUTH;
  }

  if (to === from + 1 && to % maze.width !== 0) {
    return EAST;
  }

  if (to === from - 1 && from % maze.width !== 0) {
    return WEST;
  }

  return ALL_WALLS;
}

export function validate(maze: Maze, path: readonly number[]) {
  const reaches = path[0] === maze.entrance && path[path.length - 1] === maze.exit;
  const inside = path.every((cell) => cell >= 0 && cell < maze.cells.length);
  const connected = path.length > 0 && inside && path.every((cell, position) => position === 0 || (maze.cells[path[position - 1]] & wallBetween(maze, path[position - 1], cell)) === 0);

  return { reaches, connected, valid: reaches && connected };
}

export function* replay(steps: unknown, cells: number): Generator<MazeStep> {
  if (!Array.isArray(steps)) {
    throw new Error('API steps are not a list');
  }

  for (const step of steps as MazeStep[]) {
    const knownType = step?.type === 'visit' || step?.type === 'path';

    if (!knownType || !Number.isInteger(step.index) || step.index < 0 || step.index >= cells) {
      throw new Error(`Bad step from API: ${JSON.stringify(step)}`);
    }

    yield step;
  }
}

function neighbour(maze: Maze, cell: number, side: number) {
  const next = cell + { [NORTH]: -maze.width, [EAST]: 1, [SOUTH]: maze.width, [WEST]: -1 }[side]!;

  return next >= 0 && next < maze.cells.length && wallBetween(maze, cell, next) === side ? next : OUTSIDE;
}

export function startGeneration(maze: Maze) {
  const walls = maze.cells.map(() => ALL_WALLS);
  const reached = new Set<number>();

  return {
    walls,
    apply(step: GenerationStep) {
      const next = neighbour(maze, step.cell, step.side);
      walls[step.cell] &= ~step.side;

      if (next === OUTSIDE) {
        return;
      }

      walls[next] &= ~OPPOSITE[step.side];

      if (reached.size === 0) {
        reached.add(step.cell);
      }

      if (reached.has(step.cell)) {
        reached.add(next);
      }
    },
    check() {
      const connected = reached.size === maze.cells.length;
      const matches = walls.every((cellWalls, cell) => cellWalls === maze.cells[cell]);

      return { connected, matches, valid: connected && matches };
    },
  };
}

export function* replayGeneration(steps: unknown, maze: Maze): Generator<GenerationStep> {
  if (!Array.isArray(steps)) {
    throw new Error('API steps are not a list');
  }

  for (const step of steps as GenerationStep[]) {
    const onGrid = Number.isInteger(step?.cell) && step.cell >= 0 && step.cell < maze.cells.length && OPPOSITE[step.side] !== undefined;
    const leavesGrid = onGrid && neighbour(maze, step.cell, step.side) === OUTSIDE;
    const knownType = step?.type === 'door' ? leavesGrid : (step?.type === 'carve' || step?.type === 'open') && !leavesGrid;

    if (!onGrid || !knownType) {
      throw new Error(`Bad step from API: ${JSON.stringify(step)}`);
    }

    yield step;
  }
}

export type Racer = { key: string; visits: number[]; path: number[] };

export function toRacer(key: string, steps: Iterable<MazeStep>): Racer {
  const racer: Racer = { key, visits: [], path: [] };

  for (const step of steps) {
    (step.type === 'visit' ? racer.visits : racer.path).push(step.index);
  }

  return racer;
}

export function checkRace(maze: Maze, racers: readonly Racer[], winners: readonly string[]) {
  const finishers = racers.filter((racer) => validate(maze, racer.path).valid);
  const fewestVisits = Math.min(...finishers.map((racer) => racer.visits.length));
  const fastest = finishers.filter((racer) => racer.visits.length === fewestVisits).map((racer) => racer.key);
  const valid = fastest.length === winners.length && fastest.every((key) => winners.includes(key));
  const finishLine = finishers.length > 0 ? fewestVisits : Math.max(...racers.map((racer) => racer.visits.length));

  return { valid, finishLine };
}
