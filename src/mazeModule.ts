import { api, createModule, yesNo, type Run } from './engine';
import { createMazeRace } from './mazeRace';
import { fromApi, replay, replayGeneration, SIZE, SOLVERS, startGeneration, validate, wallPath, type GenerationStep, type Maze, type MazeStep } from './maze';

const SEED_LIMIT = 2 ** 31;

type BuiltMaze = { maze: Maze; carved: number; loops: number };

export const createMaze = () => {
  let built: BuiltMaze | null = null;

  const solve = createModule<GenerationStep | MazeStep>({
    id: 'maze',
    label: 'Maze',
    cell: '02 / MAZE',
    open: '02 / MAZE GENERATOR + SOLVER',
    about: 'A visualization of how a maze is built and then solved. Watch the walls get carved out, then a randomly picked solver looks for the way from the blue entrance to the pink exit. In the Race tab five solvers share one maze: guess which one reaches the exit first.',
    tech: 'The .NET API generates the maze from a seed and runs the solver, recording every step. The page only replays those steps, then checks that every cell is reachable, the replayed walls match the served maze and the path really connects entrance and exit. Randomized Prim grows the maze from one cell by knocking down a random wall on its edge, then opens a few extra walls so there is more than one route. In the Race tab the API runs five solvers on one maze and names the one that reaches the exit in the fewest steps; the page replays them side by side and checks that verdict against the routes.',
    success: '✓ PATH FOUND',
    failure: '✕ FAILED',
    view: `<div class="maze solve-maze" style="--size:${SIZE}">${'<span></span>'.repeat(SIZE * SIZE)}<svg viewBox="0 0 ${SIZE} ${SIZE}" aria-hidden="true"><path /></svg></div>`,
    counters: [
      { key: 'c', label: 'CARVED' },
      { key: 'l', label: 'LOOPS' },
      { key: 'v', label: 'VISITED' },
      { key: 'e', label: 'EXPLORED' },
      { key: 'b', label: 'BACKTRACKS' },
      { key: 'p', label: 'PATH' },
    ],
    tickMs: 25,
    budget: Infinity,
    restart: 'New maze',
    finishPhase: 'Finish maze',
    finishRun: 'Finish run',
    newRun(element, restart): Run<GenerationStep | MazeStep> {
      const reused = restart ? null : built;
      const seed = reused?.maze.seed ?? Math.floor(Math.random() * SEED_LIMIT);
      const solver = SOLVERS[Math.floor(Math.random() * SOLVERS.length)];
      const cells = [...element.querySelectorAll<HTMLElement>('.solve-maze span')];
      const walls = element.querySelector('.solve-maze path')!;
      const doors = reused ? [reused.maze.entrance, reused.maze.exit] : [];
      const seen = new Set<number>();
      const onPath = new Set<number>();
      const path: number[] = [];
      let maze = reused?.maze ?? null;
      let generation: ReturnType<typeof startGeneration> | null = null;
      let carved = reused?.carved ?? 0;
      let loops = reused?.loops ?? 0;
      let visits = 0;
      let current = -1;
      walls.setAttribute('d', '');

      const look = (cell: number) => {
        if (cell === doors[0]) {
          return 'entrance';
        }

        if (cell === doors[1]) {
          return 'exit';
        }

        if (cell === current) {
          return 'cur';
        }

        if (onPath.has(cell)) {
          return 'path';
        }

        if (seen.has(cell)) {
          return 'seen';
        }

        return '';
      };

      const build = (step: GenerationStep) => {
        generation!.apply(step);
        current = step.type === 'door' ? -1 : step.cell;

        if (step.type === 'carve') {
          carved++;
        }

        if (step.type === 'open') {
          loops++;
        }

        if (step.type === 'door') {
          doors.push(step.cell);
        }
      };

      const solve = (step: MazeStep) => {
        if (generation && built?.maze !== maze && generation.check().valid) {
          built = { maze: maze!, carved, loops };
        }

        current = step.index;

        if (step.type === 'visit') {
          visits++;
          seen.add(step.index);
        } else {
          path.push(step.index);
          onPath.add(step.index);
        }
      };

      return {
        name: solver.name,
        big: solver.big,
        info: `${solver.name} solves it. ${solver.info}`,
        async remote(signal) {
          if (reused) {
            const { steps } = await api('/api/maze', signal, { algorithm: solver.key, seed });

            return replay(steps, reused.maze.cells.length);
          }

          const body = await api(`/api/maze/generation?seed=${seed}`, signal);
          const served = fromApi(body.maze);

          if (served.width !== SIZE || served.height !== SIZE) {
            throw new Error('Maze from API has the wrong size');
          }

          maze = served;
          generation = startGeneration(served);
          const { steps } = await api('/api/maze', signal, { algorithm: solver.key, seed: served.seed });

          return (function* () {
            yield* replayGeneration(body.steps, served);
            yield* replay(steps, served.cells.length);
          })();
        },
        onStep(step) {
          if (step.type === 'visit' || step.type === 'path') {
            solve(step);
          } else {
            build(step);
          }
        },
        check() {
          const mazeBuilt = reused !== null || generation!.check().valid;
          const route = validate(maze!, path);

          return {
            valid: mazeBuilt && route.valid,
            detail: `MAZE BUILT: ${yesNo(mazeBuilt)} · REACHES GOAL: ${yesNo(route.reaches)} · PATH CONNECTED: ${yesNo(route.connected)}`,
          };
        },
        phaseDone: () => doors.length === 2,
        counters: () => [String(carved), String(loops), String(visits), String(seen.size), String(visits - seen.size), String(path.length)],
        draw() {
          if (!maze) {
            return;
          }

          walls.setAttribute('d', wallPath(generation ? { ...maze, cells: generation.walls } : maze));
          cells.forEach((cell, index) => {
            const className = look(index);

            if (cell.className !== className) {
              cell.className = className;
            }
          });
        },
      };
    },
  });

  const race = createMazeRace();
  const element = solve.element;
  const tabs = document.createElement('div');
  tabs.className = 'maze-tabs open-only';
  tabs.setAttribute('role', 'group');
  tabs.setAttribute('aria-label', 'View');
  tabs.innerHTML = '<button type="button" data-tab="solve">Solve</button><button type="button" data-tab="race">Race</button>';
  element.querySelector('.mod-head')!.after(tabs);
  element.querySelector('.mod-actions')!.after(race.element);
  let active = false;

  function showTab(tab: string) {
    element.dataset.tab = tab;
    race.element.hidden = tab !== 'race';
    tabs.querySelectorAll<HTMLButtonElement>('[data-tab]').forEach((button) => button.setAttribute('aria-pressed', String(button.dataset.tab === tab)));
    solve.setActive(active && tab === 'solve');
    race.setActive(active && tab === 'race');
  }

  tabs.querySelectorAll<HTMLButtonElement>('[data-tab]').forEach((button) => button.addEventListener('click', () => showTab(button.dataset.tab!)));
  showTab('solve');

  return {
    element,
    setActive(isActive: boolean) {
      active = isActive;
      showTab('solve');
    },
  };
};
