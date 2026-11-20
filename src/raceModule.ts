import { api, ApiError, helpButtons, helpPopovers } from './engine';

type Mode = 'unsafe' | 'atomic' | 'queue';

interface Seat {
  x: number;
  y: number;
}

interface RaceResult {
  success: number;
  rejected: number;
  orders: number;
  stockLeft: number;
  durationMs: number;
  purchases: boolean[];
}

const API_TIMEOUT_MS = 7000;
const LOOP_MS = 2000;
const RESULT_MS = 1500;
const HOP_MS = 350;
const FLASH_MS = 300;
const PULSE_COLOR = '#eaffdc';
const GLOW = 'drop-shadow(0 0 6px rgba(178, 241, 140, 0.9))';
const CUSTOMERS = 20;
const COMICS = 1;
const CLIENT_COLUMNS = 5;
const CHANNEL_SLOTS = 6;
const ROWS = [34, 45, 56, 67];
const SVG_NS = 'http://www.w3.org/2000/svg';

const wait = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

const CONSISTENT_DETAIL: Record<Mode, string> = {
  unsafe: 'Every customer read the stock before anyone wrote it.',
  atomic: 'PostgreSQL allowed one purchase.',
  queue: 'Single consumer processed all requests sequentially.',
};

const seats: Seat[] = Array.from({ length: CUSTOMERS }, (_, index) => ({
  x: 12 + (index % CLIENT_COLUMNS) * 11,
  y: ROWS[Math.floor(index / CLIENT_COLUMNS)],
}));

const clients = seats.map(({ x, y }) => `<circle class="client" cx="${x}" cy="${y}" r="3"/>`).join('');

const fan = (targetX: number) => ROWS.map((y) => `<path class="flow" d="M60 ${y} C86 ${y} 90 52 ${targetX} 52"/>`).join('');

const routes = (route: (seat: Seat) => string) => seats.map((seat) => `<path class="route" d="${route(seat)}"/>`).join('');

const fanRoute = (end: string) => ({ x, y }: Seat) => `M${x} ${y} H60 C86 ${y} 90 52 ${end}`;

const slots = Array.from({ length: CHANNEL_SLOTS }, (_, index) => `<rect class="slot${index === 0 ? ' head' : ''}" x="${84 + index * 13}" y="46" width="10" height="12" rx="2"/>`).join('');

const icons = {
  unsafe: '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M12 3 2 20h20Z"/><path d="M12 10v4M12 17h.01"/></svg>',
  atomic: '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" aria-hidden="true"><ellipse cx="12" cy="5" rx="8" ry="3"/><path d="M4 5v14c0 1.7 3.6 3 8 3s8-1.3 8-3V5M4 12c0 1.7 3.6 3 8 3s8-1.3 8-3"/></svg>',
  queue: '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linejoin="round" aria-hidden="true"><path d="m12 3 9 5-9 5-9-5Z"/><path d="m3 12 9 5 9-5M3 16l9 5 9-5"/></svg>',
};

const markup = `
  <div class="mod-head">
    <span class="cell-only">06 / RACE</span><span class="open-only">06 / RACE CONDITION</span>
    ${helpButtons('race', 'Race Condition')}
  </div>
  <div class="mod-title open-only"><span class="alg">Limited Edition Comics</span></div>
  <p class="race-subtitle open-only">${CUSTOMERS} concurrent customers. ${COMICS} copy left. Different strategies.</p>
  <div class="race-modes open-only" role="group" aria-label="Strategy">
    <button type="button" data-mode="unsafe" aria-pressed="true">${icons.unsafe}UNSAFE</button>
    <button type="button" data-mode="atomic" aria-pressed="false">${icons.atomic}ATOMIC</button>
    <button type="button" data-mode="queue" aria-pressed="false">${icons.queue}QUEUE</button>
  </div>
  <svg class="race-flow" viewBox="0 0 320 100" role="img" aria-label="${CUSTOMERS} customers race for ${COMICS} limited edition comic">
    <defs><marker id="race-arrow" viewBox="0 0 6 6" refX="5" refY="3" markerWidth="6" markerHeight="6" orient="auto"><path d="M0 0 6 3 0 6Z"/></marker></defs>
    <g class="clients">${clients}</g>
    <text x="34" y="86" text-anchor="middle">${CUSTOMERS} CLIENTS</text>
    <g class="stage stage-unsafe">
      ${fan(118)}
      ${routes(fanRoute('118 52'))}
      <rect class="box" x="118" y="40" width="84" height="24" rx="4"/>
      <text class="box-label" x="160" y="55" text-anchor="middle">READ → WRITE</text>
      <path class="flow out" d="M202 52 H254" marker-end="url(#race-arrow)"/>
    </g>
    <g class="stage stage-atomic">
      ${fan(110)}
      ${routes(fanRoute('110 52 H126'))}
      <path class="flow" d="M110 52 H126"/>
      <rect class="tag-box" x="118" y="12" width="84" height="14" rx="3"/>
      <text x="160" y="22" text-anchor="middle">CHECK + UPDATE</text>
      <rect class="box" x="126" y="32" width="68" height="40" rx="4"/>
      <g class="icon"><ellipse cx="160" cy="42" rx="7" ry="2.5"/><path d="M153 42v10c0 1.4 3.1 2.5 7 2.5s7-1.1 7-2.5V42M153 47c0 1.4 3.1 2.5 7 2.5s7-1.1 7-2.5"/></g>
      <text class="box-label" x="160" y="66" text-anchor="middle">ATOMIC</text>
      <path class="flow out" d="M194 52 H254" marker-end="url(#race-arrow)"/>
    </g>
    <g class="stage stage-queue">
      <path class="flow" d="M62 52 H78" marker-end="url(#race-arrow)"/>
      ${routes(({ x, y }) => `M${x} ${y} L62 52 H80`)}
      <text class="channel-label" x="122" y="38" text-anchor="middle">CHANNEL</text>
      <rect class="box" x="80" y="42" width="84" height="20" rx="4"/>
      ${slots}
      <path class="flow to-worker" d="M166 52 H180" marker-end="url(#race-arrow)"/>
      <text x="199" y="32" text-anchor="middle">WORKER</text>
      <rect class="box worker" x="184" y="38" width="30" height="28" rx="4"/>
      <g class="icon"><circle cx="199" cy="52" r="5"/><circle cx="199" cy="52" r="1.5"/></g>
      <path class="flow out" d="M216 52 H254" marker-end="url(#race-arrow)"/>
    </g>
    <rect class="cover" x="270" y="32" width="30" height="40" rx="2"/>
    <path class="spine" d="M275 32 V72"/>
    <text class="comic-count" x="287.5" y="56" text-anchor="middle">${COMICS}</text>
    <text x="285" y="82" text-anchor="middle">LIMITED EDITION</text>
    <text x="285" y="92" text-anchor="middle">COMICS</text>
    <g class="particles"></g>
  </svg>
  <div class="counters open-only">
    <div><span>SUCCESS</span><b data-k="success"></b></div>
    <div><span>REJECTED</span><b data-k="rejected"></b></div>
    <div><span>ORDERS</span><b data-k="orders"></b></div>
    <div><span>STOCK</span><b data-k="stock"></b></div>
  </div>
  <div class="mod-status">
    <span class="race-line"><span class="status" role="status"></span><span class="race-chip open-only"></span></span>
    <span class="status-short" aria-hidden="true"></span>
    <span class="detail open-only"></span>
  </div>
  <div class="mod-actions open-only">
    <button type="button" class="race-btn" data-a="race">▷ Race ×${CUSTOMERS}</button>
    <button type="button" class="round" data-a="reset" aria-label="Reset" title="Reset">↻</button>
    <button type="button" data-a="trace" hidden>Trace in 09 ↗</button>
  </div>
  ${helpPopovers('race', `<p>${CUSTOMERS} customers try to buy the last ${COMICS} copy of a comic at the same moment. Pick a strategy and race them to see which one sells the same copy more than once.</p>`, `
    <p>Unsafe reads the stock, waits until all ${CUSTOMERS} customers have read it, then writes, so every one of them sees a copy left.</p>
    <p>Atomic decides in one PostgreSQL statement: UPDATE … WHERE "Stock" &gt; 0.</p>
    <p>Queue writes every request to a channel, then POST /api/race/queue/{id}/process drains it with one consumer.</p>
    <p class="info-foot">Real PostgreSQL rows · every race sells a fresh comic</p>`)}`;

export const createRaceModule = (onTrace: (runId: string) => void) => {
  const reducedMotion = matchMedia('(prefers-reduced-motion: reduce)');
  const element = document.createElement('section');
  element.className = 'mod mod-race';
  element.setAttribute('aria-label', 'Limited Edition Comics');
  element.innerHTML = markup;

  const find = <T extends Element = Element>(selector: string) => element.querySelector<T>(selector)!;
  const flow = find<SVGSVGElement>('.race-flow');
  const dots = [...element.querySelectorAll<SVGCircleElement>('.client')];
  const slotRects = [...element.querySelectorAll<SVGRectElement>('.slot')];
  const particles = find<SVGGElement>('.particles');
  const cover = find<SVGRectElement>('.cover');
  const modeButtons = [...element.querySelectorAll<HTMLButtonElement>('[data-mode]')];
  const raceButton = find<HTMLButtonElement>('[data-a=race]');
  const traceButton = find<HTMLButtonElement>('[data-a=trace]');
  const status = find('.status');
  const statusShort = find('.status-short');

  let mode: Mode = 'unsafe';
  let run = 0;
  let loop = 0;
  let loops = 0;
  let tracedRun: string | null = null;

  const isOpen = () => 'open' in element.dataset;
  const put = (key: string, text: string) => {
    find(`[data-k="${key}"]`).textContent = text;
  };

  function setStatus(text: string, short: string, detail: string, failed: boolean) {
    status.textContent = text;
    statusShort.textContent = short;
    find('.detail').textContent = detail;

    if (failed) {
      element.dataset.result = 'failed';
      return;
    }

    delete element.dataset.result;
  }

  function setState(state: string) {
    flow.dataset.state = state;
    raceButton.disabled = state === 'racing' || state === 'queued';
  }

  function setChannel(queued: number) {
    slotRects.forEach((slot, index) => slot.classList.toggle('filled', index < Math.ceil((queued * CHANNEL_SLOTS) / CUSTOMERS)));
    find('.channel-label').textContent = queued ? `CHANNEL (${queued})` : 'CHANNEL';
  }

  function clear() {
    run++;
    setState('idle');
    particles.replaceChildren();
    dots.forEach((dot) => dot.setAttribute('class', 'client'));
    setChannel(0);
    ['success', 'rejected', 'orders'].forEach((key) => put(key, '–'));
    put('stock', String(COMICS));
    find('.comic-count').textContent = String(COMICS);
    find('.race-chip').textContent = '';
    setStatus('READY', 'READY', `${CUSTOMERS} requests will hit the API at once.`, false);
  }

  function selectMode(next: Mode) {
    mode = next;
    traceButton.hidden = next !== 'queue' || !tracedRun;
    flow.dataset.mode = next;
    modeButtons.forEach((button) => button.setAttribute('aria-pressed', String(button.dataset.mode === next)));
    clear();
  }

  function showResult(result: RaceResult) {
    const consistent = result.success === result.orders && result.orders + result.stockLeft === COMICS;
    setState(consistent ? 'consistent' : 'oversold');
    result.purchases.forEach((bought, index) => dots[index].setAttribute('class', `client ${bought ? 'won' : 'lost'}`));
    put('success', String(result.success));
    put('rejected', String(result.rejected));
    put('orders', String(result.orders));
    put('stock', String(result.stockLeft));
    find('.comic-count').textContent = String(result.orders);
    find('.race-chip').textContent = `${result.durationMs.toFixed(1)} ms`;

    if (consistent) {
      setStatus('CONSISTENT', 'CONSISTENT', CONSISTENT_DETAIL[mode], false);
      return;
    }

    setStatus(`OVERSOLD: ${result.orders} ORDERS FOR ${COMICS} COMIC`, 'OVERSOLD', 'Race condition reproduced. Inventory is inconsistent.', true);
  }

  function hop(line: SVGPathElement, current: number) {
    if (reducedMotion.matches || current !== run) {
      return Promise.resolve();
    }

    const length = line.getTotalLength();
    const start = line.getPointAtLength(0);
    const particle = document.createElementNS(SVG_NS, 'circle');
    particle.setAttribute('r', '2.5');
    particle.setAttribute('cx', String(start.x));
    particle.setAttribute('cy', String(start.y));
    particle.classList.add('particle');
    particles.append(particle);

    return new Promise<void>((resolve) => {
      const startedAt = performance.now();

      const frame = (now: number) => {
        const progress = Math.min(1, (now - startedAt) / HOP_MS);
        const point = line.getPointAtLength(progress * length);
        particle.setAttribute('cx', String(point.x));
        particle.setAttribute('cy', String(point.y));

        if (progress < 1 && current === run) {
          requestAnimationFrame(frame);
          return;
        }

        particle.remove();
        resolve();
      };

      requestAnimationFrame(frame);
    });
  }

  function flash(target: Element, current: number) {
    if (current !== run) {
      return Promise.resolve();
    }

    if (!reducedMotion.matches) {
      target.animate([{ stroke: PULSE_COLOR, filter: GLOW }, {}], { duration: FLASH_MS });
    }

    return wait(FLASH_MS);
  }

  function sendEveryone(stage: Element, current: number) {
    dots.forEach((dot) => dot.classList.add('sent'));
    const lines = [...stage.querySelectorAll<SVGPathElement>('.route')];

    return Promise.all(lines.map((line) => hop(line, current)));
  }

  async function play(current: number, signal: AbortSignal): Promise<RaceResult> {
    const stage = find(`.stage-${mode}`);
    const box = stage.querySelector('.box')!;
    let result: RaceResult;

    if (mode === 'queue') {
      const [queued] = await Promise.all([api('/api/race/queue', signal, {}), sendEveryone(stage, current)]);
      tracedRun = queued.runId;
      traceButton.hidden = false;

      if (current === run) {
        setChannel(queued.queued);
        setState('queued');
        setStatus(`QUEUED ${queued.queued} ORDERS`, 'QUEUED', 'POST /api/race/queue/{id}/process starts one worker.', false);
      }

      [result] = await Promise.all([api(`/api/race/queue/${queued.runId}/process`, signal, {}), flash(box, current)]);

      if (current === run) {
        setChannel(queued.queued - 1);
        slotRects[0].classList.remove('filled');
      }

      await hop(stage.querySelector<SVGPathElement>('.to-worker')!, current);
      await flash(stage.querySelector('.worker')!, current);
    } else {
      [result] = await Promise.all([api(`/api/race/${mode}`, signal, {}), sendEveryone(stage, current)]);
      await flash(box, current);
    }

    await hop(stage.querySelector<SVGPathElement>('.out')!, current);
    await flash(cover, current);

    return result;
  }

  async function race() {
    clear();
    const current = run;
    setState('racing');
    setStatus('RACING…', 'RACING', `${CUSTOMERS} customers want ${COMICS} comic.`, false);

    try {
      const result = await play(current, AbortSignal.timeout(API_TIMEOUT_MS));

      if (current === run) {
        showResult(result);
      }
    } catch (error) {
      if (current === run) {
        setState('idle');
        setStatus(error instanceof ApiError && error.detail ? error.detail : '✕ API UNREACHABLE', '✕ FAILED', '', true);
      }
    }
  }

  async function loopRaces(id: number) {
    while (id === loop) {
      const startedAt = performance.now();
      await race();
      await wait(RESULT_MS);

      if (id !== loop) {
        return;
      }

      clear();
      await wait(LOOP_MS - (performance.now() - startedAt));
    }
  }

  modeButtons.forEach((button) => button.addEventListener('click', () => selectMode(button.dataset.mode as Mode)));
  raceButton.addEventListener('click', race);
  find('[data-a=reset]').addEventListener('click', clear);
  traceButton.addEventListener('click', () => onTrace(tracedRun!));

  selectMode(mode);

  return {
    element,
    setActive(active: boolean) {
      if (active && !isOpen() && !loop) {
        loop = ++loops;
        loopRaces(loop);
      }

      if (!active && loop) {
        loop = 0;
        clear();
      }
    },
  };
};
