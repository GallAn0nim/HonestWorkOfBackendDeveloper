import { api, createSpeed, speedControl } from './engine';
import { checkRace, fromApi, replay, SIZE, SOLVERS, toRacer, wallPath, type Maze, type Racer, type Solver } from './maze';

const RACER_COUNT = 5;
const TICK_MS = 25;
const API_TIMEOUT_MS = 2000;
const COLORS = ['#ffc21a', '#9b5cff', '#14e0c8', '#ff6a1a', '#ff3d5a'];

type Entrant = Racer & { solver: Solver; color: string };

const markup = `
  <div class="maze race-maze" style="--size:${SIZE}">${'<span></span>'.repeat(SIZE * SIZE)}<svg viewBox="0 0 ${SIZE} ${SIZE}" aria-hidden="true"><path /></svg></div>
  <p class="race-ask">Who wins? Pick one.</p>
  <div class="race-picks" role="group" aria-label="Pick the winner"></div>
  <div class="mod-status"><span class="race-status" role="status"></span></div>
  <div class="mod-actions">
    <button type="button" data-race="finish">Finish race</button>
    <button type="button" data-race="again">Race again</button>
  </div>
  ${speedControl}`;

function pickSolvers() {
  const pool = [...SOLVERS];

  return Array.from({ length: RACER_COUNT }, () => pool.splice(Math.floor(Math.random() * pool.length), 1)[0]);
}

const names = (entrants: Entrant[]) => entrants.map((entrant) => entrant.solver.name).join(' and ');

export function createMazeRace() {
  const reducedMotion = matchMedia('(prefers-reduced-motion: reduce)');
  const element = document.createElement('div');
  element.className = 'maze-race open-only';
  element.innerHTML = markup;

  const cells = [...element.querySelectorAll<HTMLElement>('.race-maze span')];
  const walls = element.querySelector('.race-maze path')!;
  const picks = element.querySelector<HTMLElement>('.race-picks')!;
  const status = element.querySelector<HTMLElement>('.race-status')!;
  const finishButton = element.querySelector<HTMLButtonElement>('[data-race=finish]')!;

  let maze: Maze | null = null;
  let entrants: Entrant[] = [];
  let winners: Entrant[] = [];
  let agreed = false;
  let finishLine = 0;
  let guess: string | null = null;
  let tick = 0;
  let finished = false;
  let raceId = 0;
  let active = false;
  let timer = 0;
  const speed = createSpeed(element.querySelector('.speed')!, () => {
    clearInterval(timer);
    timer = 0;
    sync();
  });

  function setStatus(text: string, result = '') {
    status.textContent = text;
    element.dataset.result = result;
  }

  async function load() {
    const loadId = ++raceId;
    const solvers = pickSolvers();
    maze = null;
    entrants = [];
    guess = null;
    tick = 0;
    finished = false;
    walls.setAttribute('d', '');
    picks.replaceChildren();
    setStatus('LOADING…');
    draw();
    sync();

    try {
      const body = await api('/api/maze/race', AbortSignal.timeout(API_TIMEOUT_MS), { algorithms: solvers.map((solver) => solver.key) });

      if (loadId !== raceId) {
        return;
      }

      const served = fromApi(body.maze);

      if (served.width !== SIZE || served.height !== SIZE || !Array.isArray(body.racers) || !Array.isArray(body.winners)) {
        throw new Error(`Bad race from API: ${JSON.stringify(body)}`);
      }

      const servedEntrants: Entrant[] = solvers.map((solver, index) => {
        const racer = body.racers[index];

        if (racer?.algorithm !== solver.key) {
          throw new Error(`Race from API is missing ${solver.key}`);
        }

        return { ...toRacer(solver.key, replay(racer.steps, served.cells.length)), solver, color: COLORS[index] };
      });

      maze = served;
      entrants = servedEntrants;
      winners = entrants.filter((entrant) => body.winners.includes(entrant.key));
      ({ valid: agreed, finishLine } = checkRace(served, entrants, body.winners));
    } catch (error) {
      if (loadId !== raceId) {
        return;
      }

      console.error(error);
      setStatus(error instanceof TypeError || error instanceof DOMException ? '✕ API UNREACHABLE' : '✕ ERROR', 'failed');
      return;
    }

    walls.setAttribute('d', wallPath(maze));
    picks.replaceChildren(...entrants.map(pickButton));
    setStatus('');
    draw();
  }

  function pickButton(entrant: Entrant) {
    const button = document.createElement('button');
    button.type = 'button';
    button.dataset.key = entrant.key;
    button.style.setProperty('--c', entrant.color);
    button.textContent = entrant.solver.name;
    button.addEventListener('click', () => start(entrant.key));

    return button;
  }

  function start(key: string) {
    guess = key;
    picks.querySelectorAll('button').forEach((button) => {
      button.disabled = true;
    });
    setStatus('● RACING');

    if (reducedMotion.matches) {
      runToFinish();
      return;
    }

    sync();
  }

  function step() {
    tick++;

    if (tick >= finishLine) {
      finish();
    }
  }

  function runToFinish() {
    while (!finished) {
      step();
    }

    draw();
  }

  function finish() {
    finished = true;
    sync();

    picks.querySelectorAll<HTMLButtonElement>('button').forEach((button) => {
      const won = winners.some((winner) => winner.key === button.dataset.key);
      button.toggleAttribute('data-won', won);

      if (button.dataset.key === guess) {
        button.dataset.verdict = won ? 'right' : 'wrong';
      }
    });

    if (!agreed) {
      setStatus('✕ CONTRACT FAILED', 'failed');
      return;
    }

    if (winners.length === 0) {
      setStatus('Nobody reached the exit.', 'failed');
      return;
    }

    const result = `${names(winners)} ${winners.length > 1 ? 'tied' : 'won'} in ${finishLine} steps.`;
    const guessedRight = winners.some((winner) => winner.key === guess);
    setStatus(guessedRight ? `✓ You guessed it! ${result}` : `✕ ${result}`, guessedRight ? 'valid' : 'failed');
  }

  function draw() {
    const colors = new Map<number, string>();
    const heads = new Set<number>();
    const reachedAt = new Map<number, number>();

    entrants.forEach((entrant) => {
      entrant.visits.slice(0, tick).forEach((cell, step) => {
        if (step < (reachedAt.get(cell) ?? Infinity)) {
          reachedAt.set(cell, step);
          colors.set(cell, entrant.color);
        }
      });
    });

    entrants.forEach((entrant) => {
      const head = entrant.visits[Math.min(tick, entrant.visits.length) - 1];

      if (head !== undefined) {
        colors.set(head, entrant.color);
        heads.add(head);
      }
    });

    const mine = entrants.find((entrant) => entrant.key === guess);
    const myHead = mine?.visits[Math.min(tick, mine.visits.length) - 1];
    const route = new Map<number, string>();

    if (finished && agreed) {
      winners.forEach((winner) => winner.path.forEach((cell) => route.set(cell, winner.color)));
    }

    cells.forEach((cell, index) => {
      let className = '';

      if (index === maze?.entrance) {
        className = 'entrance';
      } else if (index === maze?.exit) {
        className = 'exit';
      } else if (route.has(index)) {
        className = 'route';
      } else if (heads.has(index)) {
        className = 'head';
      } else if (colors.has(index)) {
        className = 'trail';
      }

      const color = route.get(index) ?? colors.get(index) ?? '';

      if (cell.className !== className) {
        cell.className = className;
      }

      cell.classList.toggle('mine', index === myHead);

      if (cell.style.getPropertyValue('--c') !== color) {
        cell.style.setProperty('--c', color);
      }
    });

    finishButton.disabled = !guess || finished;
  }

  function sync() {
    const play = active && guess !== null && !finished && !reducedMotion.matches;

    if (play && !timer) {
      timer = setInterval(() => {
        step();
        draw();
      }, speed.interval(TICK_MS));
    }

    if (!play && timer) {
      clearInterval(timer);
      timer = 0;
    }
  }

  element.querySelector('[data-race=finish]')!.addEventListener('click', runToFinish);
  element.querySelector('[data-race=again]')!.addEventListener('click', load);
  reducedMotion.addEventListener('change', sync);

  return {
    element,
    setActive(isActive: boolean) {
      active = isActive;

      if (active && raceId === 0) {
        load();
      }

      sync();
    },
  };
}
