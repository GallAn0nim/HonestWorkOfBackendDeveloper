export const STEP_BUDGET = 5_000;
export const yesNo = (value: boolean) => (value ? 'YES' : 'NO');
const API_TIMEOUT_MS = 2000;
const NEXT_RUN_DELAY_MS = 1500;
const SPEEDS = [0.25, 0.5, 1, 2, 4, 8];
const NORMAL_SPEED = SPEEDS.indexOf(1);

export interface Run<Step> {
  name: string;
  big: string;
  info: string;
  remote(signal: AbortSignal): Promise<Generator<Step>>;
  onStep(step: Step): void;
  check(): { valid: boolean; detail: string };
  counters(): string[];
  draw(): void;
  phaseDone?(): boolean;
}

export interface ModuleSpec<Step> {
  id: string;
  label: string;
  cell: string;
  open: string;
  about: string;
  tech: string;
  success: string;
  failure?: string;
  view: string;
  counters: { key: string; label: string }[];
  tickMs: number;
  budget?: number;
  restart?: string;
  finishPhase?: string;
  finishRun?: string;
  repeats?: boolean;
  newRun(element: HTMLElement, restart: boolean): Run<Step>;
}

const ENDINGS = {
  valid: { text: '✓ VALID', short: '✓ VALID', result: 'valid' },
  failed: { text: '✕ CONTRACT FAILED', short: '✕ FAILED', result: 'failed' },
  budget: { text: 'STOPPED: budget', short: 'STOPPED', result: 'failed' },
  error: { text: '✕ ERROR', short: '✕ ERROR', result: 'failed' },
  offline: { text: '✕ API UNREACHABLE', short: '✕ OFFLINE', result: 'failed' },
} as const;
type Ending = { text: string; short: string; result: string };

export function advance<Step>(steps: Generator<Step>, onStep: (step: Step) => void, state: { steps: number }, budget = STEP_BUDGET): 'step' | 'done' | 'budget' {
  const next = steps.next();

  if (next.done) {
    return 'done';
  }

  onStep(next.value);
  state.steps++;

  return state.steps >= budget ? 'budget' : 'step';
}

export class ApiError extends Error {
  readonly status: number;
  readonly detail?: string;

  constructor(status: number, body: { errors?: Record<string, string[]> } | null) {
    super(`API ${status}`);
    this.status = status;
    this.detail = Object.values(body?.errors ?? {})[0]?.[0];
  }
}

export async function api(path: string, signal: AbortSignal, body?: unknown): Promise<any> {
  const response = await fetch(path, {
    method: body === undefined ? 'GET' : 'POST',
    headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
    body: body === undefined ? undefined : JSON.stringify(body),
    signal,
  });

  if (!response.ok) {
    throw new ApiError(response.status, await response.json().catch(() => null));
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

function loadSteps<Step>(run: Run<Step>) {
  return run.remote(AbortSignal.timeout(API_TIMEOUT_MS)).then(
    (steps) => ({ steps, source: 'Ran on the .NET API.' }),
    () => ({ steps: null, source: 'The .NET API is unreachable.' }),
  );
}

const helpPopover = (id: string, body: string) => `
  <div id="${id}" class="info" popover>
    <button type="button" class="round" popovertarget="${id}" popovertargetaction="hide" aria-label="Close info">✕</button>
    ${body}
  </div>`;

export const helpButtons = (id: string, label: string) => `
  <span class="help open-only">
    <button type="button" class="round info-btn" popovertarget="${id}-about" aria-label="About ${label}">i</button>
    <button type="button" class="round tech-btn" popovertarget="${id}-info" aria-label="How ${label} works">t</button>
  </span>`;

export const helpPopovers = (id: string, about: string, tech: string) => helpPopover(`${id}-about`, about) + helpPopover(`${id}-info`, tech);

export const speedControl = `
  <div class="speed open-only" role="group" aria-label="Animation speed">
    <button type="button" class="round" data-speed="-1" aria-label="Slower">−</button>
    <output class="speed-value">1×</output>
    <button type="button" class="round" data-speed="1" aria-label="Faster">+</button>
  </div>`;

export function createSpeed(control: HTMLElement, onChange: () => void) {
  const value = control.querySelector('output')!;
  const buttons = control.querySelectorAll<HTMLButtonElement>('[data-speed]');
  let index = NORMAL_SPEED;

  function show() {
    value.textContent = `${SPEEDS[index]}×`;
    buttons.forEach((button) => {
      const next = index + Number(button.dataset.speed);
      button.disabled = next < 0 || next >= SPEEDS.length;
    });
  }

  buttons.forEach((button) => {
    button.addEventListener('click', () => {
      index += Number(button.dataset.speed);
      show();
      onChange();
    });
  });
  show();

  return { interval: (baseMs: number) => baseMs / SPEEDS[index] };
}

const frame = (spec: ModuleSpec<unknown>) => `
  <div class="mod-head">
    <span class="cell-only">${spec.cell}</span><span class="open-only">${spec.open}</span>
    ${helpButtons(spec.id, spec.label)}
  </div>
  <div class="mod-title"><span class="alg"></span><span class="open-only big"></span></div>
  ${spec.view}
  <div class="counters open-only">${spec.counters.map((counter) => `<div><span>${counter.label}</span><b data-k="${counter.key}"></b></div>`).join('')}</div>
  <div class="mod-status"><span class="status" role="status"></span><span class="status-short" aria-hidden="true"></span><span class="detail open-only"></span></div>
  <div class="mod-actions open-only">
    <button type="button" class="rm-only" data-a="step">Step</button>
    ${spec.finishPhase ? `<button type="button" data-a="phase">${spec.finishPhase}</button>` : ''}
    <button type="button" class="${spec.finishRun ? '' : 'rm-only'}" data-a="end">${spec.finishRun ?? 'Run to end'}</button>
    ${spec.restart ? `<button type="button" data-a="restart">${spec.restart}</button>` : ''}
    <button type="button" data-a="again">Run another</button>
  </div>
  ${speedControl}
  ${helpPopovers(spec.id, `<p>${spec.about}</p>`, `
    <p>${spec.tech}</p>
    <p class="info-text"></p>
    <p class="info-src"></p>
    <p class="info-foot">Real operations · playback speed is not a benchmark</p>`)}`;

export function createModule<Step>(spec: ModuleSpec<Step>) {
  const reducedMotion = matchMedia('(prefers-reduced-motion: reduce)');
  const element = document.createElement('section');
  element.className = `mod mod-${spec.id}`;
  element.setAttribute('aria-label', spec.label);
  element.innerHTML = frame(spec as ModuleSpec<unknown>);

  const find = <T extends Element>(selector: string) => element.querySelector<T>(selector)!;

  const put = (selector: string, text: string) => {
    const target = find(selector);

    if (target.textContent !== text) {
      target.textContent = text;
    }
  };

  let run: Run<Step>;
  let state = { steps: 0 };
  let steps: Generator<Step> | null = null;
  let loading: Promise<void> | null = null;
  let ending: Ending | null = null;
  let runId = 0;
  let detail = '';
  let active = false;
  let timer = 0;
  const speed = createSpeed(find('.speed'), changeSpeed);

  function newRun(restart = false) {
    run = spec.newRun(element, restart);
    state = { steps: 0 };
    steps = null;
    loading = null;
    ending = null;
    detail = '';
    runId++;
    delete element.dataset.result;
    put('.alg', run.name);
    put('.big', run.big);
    put('.info-text', run.info);
    put('.info-src', '');
    find('.tech-btn').setAttribute('aria-label', `How ${run.name} works`);
    sync();
  }

  function ready(): Promise<boolean> {
    const requestedRun = runId;

    loading ??= loadSteps(run).then((loaded) => {
      if (requestedRun !== runId) {
        return;
      }

      steps = loaded.steps;
      put('.info-src', loaded.source);

      if (!loaded.steps) {
        finish(ENDINGS.offline);
      }
    });

    return loading.then(() => requestedRun === runId);
  }

  function finish(reached: Ending) {
    ending = reached;
    element.dataset.result = reached.result;
    sync();

    if (reached.result === 'valid') {
      startNextRunInList(runId);
    }
  }

  function startNextRunInList(finishedRun: number) {
    if (spec.repeats === false) {
      return;
    }

    setTimeout(() => {
      if (finishedRun === runId && active && !('open' in element.dataset)) {
        newRun();
      }
    }, NEXT_RUN_DELAY_MS);
  }

  function step() {
    if (ending || !steps) {
      return;
    }

    try {
      const outcome = advance(steps, run.onStep, state, spec.budget);

      if (outcome === 'budget') {
        finish(ENDINGS.budget);
        return;
      }

      if (outcome === 'done') {
        const verdict = run.check();
        detail = verdict.detail;
        finish(verdict.valid ? { ...ENDINGS.valid, text: spec.success } : { ...ENDINGS.failed, text: spec.failure ?? ENDINGS.failed.text });
      }
    } catch (error) {
      console.error(error);
      finish(ENDINGS.error);
    }
  }

  function playback() {
    if (timer) {
      return 'RUNNING';
    }

    return state.steps ? 'PAUSED' : 'IDLE';
  }

  function render() {
    run.draw();
    run.counters().forEach((value, index) => put(`[data-k="${spec.counters[index].key}"]`, value));
    const word = playback();
    put('.status', ending?.text ?? (timer ? `● ${word}` : word));
    put('.status-short', ending?.short ?? word);
    put('.detail', detail);

    if (spec.finishPhase) {
      find<HTMLButtonElement>('[data-a=phase]').disabled = Boolean(ending) || run.phaseDone!();
    }
  }

  function tick() {
    if (steps) {
      step();
    } else {
      ready();
    }

    render();
  }

  function sync() {
    const play = active && !ending && !reducedMotion.matches;

    if (play && !timer) {
      timer = setInterval(tick, speed.interval(spec.tickMs));
    }

    if (!play && timer) {
      clearInterval(timer);
      timer = 0;
    }

    render();
  }

  function changeSpeed() {
    clearInterval(timer);
    timer = 0;
    sync();
  }

  const whenReady = (action: () => void) => ready().then((sameRun) => {
    if (sameRun) {
      action();
      render();
    }
  });

  element.addEventListener('click', (event) => {
    const action = (event.target as HTMLElement).closest<HTMLElement>('[data-a]')?.dataset.a;

    if (action === 'again') {
      newRun();
    }

    if (action === 'restart') {
      newRun(true);
    }

    if (action === 'step') {
      whenReady(step);
    }

    if (action === 'phase') {
      whenReady(() => {
        while (!ending && !run.phaseDone!()) {
          step();
        }
      });
    }

    if (action === 'end') {
      whenReady(() => {
        while (!ending) {
          step();
        }
      });
    }
  });
  reducedMotion.addEventListener('change', sync);

  newRun();

  return {
    element,
    setActive(isActive: boolean) {
      active = isActive;

      if (isActive && ending?.result === 'valid') {
        startNextRunInList(runId);
      }

      if (!isActive) {
        find<HTMLElement>(`#${spec.id}-about`).hidePopover?.();
        find<HTMLElement>(`#${spec.id}-info`).hidePopover?.();
      }

      sync();
    },
  };
}
