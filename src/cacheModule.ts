import { api, ApiError, helpButtons, helpPopovers } from './engine';

const API_TIMEOUT_MS = 7000;
const AUTO_SEND_MS = 1000;
const HOP_MS = 280;
const BURST_SIZE = 5;
const PULSE_COLOR = '#eaffdc';
const SVG_NS = 'http://www.w3.org/2000/svg';

const SOURCE_LINE = 'M40 70 L146 70';
const POSTGRESQL_LINE = 'M154 70 C190 70 200 32 232 32';
const CACHE_LINE = 'M154 70 C190 70 200 106 226 106';

interface Metrics {
  rps: number;
  cacheActive: boolean;
  requests: number;
  queries: number;
}

interface Reply {
  source: string;
  cacheStatus: string;
  latencyMs: number;
  metrics: Metrics;
}

const layer = (offset: number, y: number) => `<polygon class="layer" points="${232 + offset},${y} ${260 + offset},${y - 7} ${288 + offset},${y} ${260 + offset},${y + 7}"/>`;

const markup = `
  <div class="mod-head">
    <span class="cell-only">04 / CACHE</span><span class="open-only">04 / ADAPTIVE CACHE</span>
    ${helpButtons('cache', 'Adaptive Cache')}
  </div>
  <div class="mod-title"><span class="alg open-only">PostgreSQL → In-Memory</span></div>
  <svg class="cache-flow" viewBox="0 0 320 140" role="img" aria-label="Requests flow from a source to PostgreSQL or to the in-memory cache">
    <path class="line line-source" d="${SOURCE_LINE}"/>
    <path class="line line-postgresql" d="${POSTGRESQL_LINE}"/>
    <path class="line line-cache" d="${CACHE_LINE}"/>
    <line class="threshold-stem" x1="150" y1="30" x2="150" y2="66"/>
    <g class="threshold"><rect x="108" y="10" width="84" height="20" rx="4"/><text x="150" y="23.5" text-anchor="middle">SWITCH @ 4 RPS</text></g>
    <circle class="junction" cx="150" cy="70" r="4"/>
    <g class="send" data-a="send" role="button" tabindex="0" aria-label="Send request"><circle cx="22" cy="70" r="18"/><text x="22" y="46" text-anchor="middle" class="tag">SEND</text></g>
    <g class="database"><ellipse cx="260" cy="22" rx="16" ry="6"/><path d="M244 22v20c0 3.3 7.2 6 16 6s16-2.7 16-6V22M244 32c0 3.3 7.2 6 16 6s16-2.7 16-6"/><text x="260" y="62" text-anchor="middle" class="tag">POSTGRESQL</text></g>
    <g class="stack">${layer(0, 92)}${layer(4, 103)}${layer(0, 114)}<text x="260" y="134" text-anchor="middle" class="tag">CACHE (MEMORY)</text></g>
    <g class="action open-only" data-a="burst" role="button" tabindex="0" aria-label="Burst ×${BURST_SIZE}"><rect x="-8" y="92" width="60" height="14"/><text x="22" y="102" text-anchor="middle">BURST ×${BURST_SIZE}</text></g>
    <g class="action open-only" data-a="reset" role="button" tabindex="0" aria-label="Reset"><rect x="-8" y="108" width="60" height="14"/><text x="22" y="118" text-anchor="middle">RESET</text></g>
    <g class="particles"></g>
  </svg>
  <div class="cache-metrics open-only">
    <div><span>SOURCE</span><b data-k="source">–</b></div>
    <div><span>LATENCY</span><b data-k="latency">–</b></div>
    <div><span>RPS</span><b data-k="rps">0</b></div>
    <b class="cache-badge" data-k="cache">DISABLED</b>
  </div>
  <div class="cache-metrics cache-totals open-only">
    <div><span>REQUESTS</span><b data-k="requests">0</b></div>
    <div><span>DB QUERIES</span><b data-k="queries">0</b></div>
  </div>
  <div class="mod-status"><span class="status" role="status"></span><span class="status-short" aria-hidden="true"></span></div>
  ${helpPopovers('cache', '<p>Shows how a cache takes load off a database. Send requests slowly and every answer comes from PostgreSQL. Send more than 4 per second, or a burst, and the API switches to an in-memory cache. Watch the latency and the DB query count.</p>', `
    <p>Every request runs a sales report in PostgreSQL: orders joined with products and categories, grouped and summed.</p>
    <p>The API counts requests per second for your session. Above the threshold it serves the report through .NET HybridCache, in memory only, for 30 seconds, so only a cache miss touches the database.</p>
    <p>At most 4 queries hit the database at once, and an idle session is dropped after 2 minutes.</p>
    <p class="info-foot">Real PostgreSQL queries · latency measured by the API</p>`)}`;

export const createCacheModule = () => {
  const reducedMotion = matchMedia('(prefers-reduced-motion: reduce)');
  const element = document.createElement('section');
  element.className = 'mod mod-cache';
  element.setAttribute('aria-label', 'Adaptive Cache');
  element.innerHTML = markup;

  const find = <T extends Element = Element>(selector: string) => element.querySelector<T>(selector)!;
  const flow = find<SVGSVGElement>('.cache-flow');
  const sourceLine = find<SVGPathElement>('.line-source');
  const postgresqlLine = find<SVGPathElement>('.line-postgresql');
  const cacheLine = find<SVGPathElement>('.line-cache');
  const particles = find<SVGGElement>('.particles');
  const layers = [...element.querySelectorAll<SVGElement>('.layer')];
  const status = find('.status');
  const statusShort = find('.status-short');

  let sessionId = crypto.randomUUID();
  let timer = 0;

  const isOpen = () => 'open' in element.dataset;
  const put = (key: string, text: string) => {
    find(`[data-k="${key}"]`).textContent = text;
  };

  function setStatus(text: string, failed = false) {
    status.textContent = text;
    statusShort.textContent = text;

    if (failed) {
      element.dataset.result = 'failed';
      return;
    }

    delete element.dataset.result;
  }

  function showMetrics(metrics: Metrics) {
    flow.dataset.cache = metrics.cacheActive ? 'on' : 'off';

    if (!metrics.cacheActive && flow.dataset.source) {
      flow.dataset.source = 'postgresql';
      put('cache', 'DISABLED');
      find<HTMLElement>('.cache-badge').dataset.state = 'disabled';
      setStatus('POSTGRESQL DIRECT');
    }

    put('rps', String(metrics.rps));
    put('requests', String(metrics.requests));
    put('queries', String(metrics.queries));
  }

  function showReply(reply: Reply) {
    showMetrics(reply.metrics);
    flow.dataset.source = reply.source;
    flow.dataset.started = '';
    put('source', reply.source === 'cache' ? 'CACHE' : 'POSTGRESQL');
    put('latency', `${reply.latencyMs < 1 ? reply.latencyMs.toFixed(2) : reply.latencyMs.toFixed(1)} ms`);
    put('cache', reply.cacheStatus.toUpperCase());
    find<HTMLElement>('.cache-badge').dataset.state = reply.cacheStatus;
    setStatus(reply.metrics.cacheActive ? 'CACHE ACTIVE' : 'POSTGRESQL DIRECT');
  }

  function clear() {
    showMetrics({ rps: 0, cacheActive: false, requests: 0, queries: 0 });
    delete flow.dataset.source;
    delete flow.dataset.started;
    ['source', 'latency'].forEach((key) => put(key, '–'));
    put('cache', 'DISABLED');
    find<HTMLElement>('.cache-badge').dataset.state = 'disabled';
    setStatus('');
  }

  function fail(error: unknown) {
    setStatus(error instanceof ApiError && error.detail ? error.detail : '✕ API UNREACHABLE', true);
  }

  function hop(particle: SVGCircleElement, line: SVGPathElement) {
    if (reducedMotion.matches) {
      return Promise.resolve();
    }

    return new Promise<void>((resolve) => {
      const length = line.getTotalLength();
      const startedAt = performance.now();

      const frame = (now: number) => {
        const progress = Math.min(1, (now - startedAt) / HOP_MS);
        const point = line.getPointAtLength(progress * length);
        particle.setAttribute('cx', String(point.x));
        particle.setAttribute('cy', String(point.y));

        if (progress < 1) {
          requestAnimationFrame(frame);
          return;
        }

        resolve();
      };

      requestAnimationFrame(frame);
    });
  }

  function pulse() {
    if (reducedMotion.matches) {
      return;
    }

    layers.forEach((item, index) => {
      item.animate([{ stroke: PULSE_COLOR, filter: 'drop-shadow(0 0 6px rgba(178, 241, 140, 0.9))' }, {}], { duration: 450, delay: index * 90, fill: 'backwards' });
    });
  }

  async function send() {
    const particle = document.createElementNS(SVG_NS, 'circle');
    particle.setAttribute('r', '2.5');
    particle.setAttribute('cx', '40');
    particle.setAttribute('cy', '70');
    particle.classList.add('particle');
    particles.append(particle);
    const session = sessionId;
    const reply: Promise<Reply> = api(`/api/cache/${session}/requests`, AbortSignal.timeout(API_TIMEOUT_MS), {});

    try {
      const [, body] = await Promise.all([hop(particle, sourceLine), reply]);

      if (session !== sessionId) {
        return;
      }

      showReply(body);
      await hop(particle, body.source === 'cache' ? cacheLine : postgresqlLine);

      if (body.cacheStatus === 'hit') {
        pulse();
      }
    } catch (error) {
      fail(error);
    } finally {
      particle.remove();
    }
  }

  async function reset() {
    const previous = sessionId;
    sessionId = crypto.randomUUID();
    clear();

    try {
      await api(`/api/cache/${previous}/reset`, AbortSignal.timeout(API_TIMEOUT_MS), {});
    } catch (error) {
      fail(error);
    }
  }

  const sendBurst = () => Promise.all(Array.from({ length: BURST_SIZE }, send));

  const actions: Record<string, () => unknown> = { send, burst: sendBurst, reset };

  function runAction(event: Event) {
    const action = (event.target as Element).closest<SVGElement>('[data-a]')?.dataset.a;

    if (!action || !isOpen()) {
      return;
    }

    event.stopPropagation();
    actions[action]();
  }

  element.addEventListener('click', runAction);
  element.addEventListener('keydown', (event) => {
    const { key } = event as KeyboardEvent;

    if (key === 'Enter' || key === ' ') {
      event.preventDefault();
      runAction(event);
    }
  });

  clear();

  return {
    element,
    setActive(active: boolean) {
      if (active && !timer) {
        timer = setInterval(send, AUTO_SEND_MS);
      }

      if (!active && timer) {
        clearInterval(timer);
        timer = 0;
      }
    },
  };
};
