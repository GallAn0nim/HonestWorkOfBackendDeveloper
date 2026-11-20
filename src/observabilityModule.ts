import { api, helpButtons, helpPopovers } from './engine';

type Level = 'INFO' | 'WARN' | 'ERROR';
type TraceStatus = 'SUCCESS' | 'REJECTED' | 'PROCESSING' | 'QUEUED' | 'FAILED';
type StageState = 'idle' | 'active' | 'done' | 'failed';

interface LogEntry {
  id: number;
  timestamp: string;
  level: Level;
  module: string;
  message: string;
  traceId?: string;
  spanId?: string;
}

interface EndpointMetrics {
  method: string;
  route: string;
  requests: number;
  rps: number;
  p50Ms?: number;
  p95Ms?: number;
  errorRate?: number;
  rpsSeries?: number[];
}

interface TraceSpan {
  traceId: string;
  spanId: string;
  parentSpanId?: string;
  linkedSpanId?: string;
  name: string;
  source: string;
  customer?: number;
  start: string;
  durationMs?: number;
  queueWaitMs?: number;
  outcome?: string;
  failed: boolean;
}

interface TraceRun {
  runId: string;
  startedAt: string;
  requests: number;
  finished: number;
}

interface TraceDetail {
  runId: string;
  spans: TraceSpan[];
  logs: LogEntry[];
}

interface Journey {
  customer: number;
  request?: TraceSpan;
  enqueue?: TraceSpan;
  processRequest?: TraceSpan;
  worker?: TraceSpan;
  database: TraceSpan[];
}

interface Bar {
  label: string;
  source: string;
  depth: number;
  start: number;
  durationMs?: number;
  kind: 'span' | 'wait' | 'outcome';
  failed?: boolean;
  tone?: string;
}

interface MetricsSnapshot {
  overview: { requests: number; rps: number; averageMs?: number; p95Ms?: number; errorRate?: number };
  rps: number[];
  endpoints: EndpointMetrics[];
}

const LOGS_URL = '/api/observability/logs';
const LOG_HISTORY_URL = '/api/observability/logs/history';
const LOG_PAGE_SIZE = 50;
const OLDER_SLACK_PX = 120;
const METRICS_URL = '/api/observability/metrics';
const POLL_MS = 2000;
const MAX_ROWS = 500;
const FOLLOW_SLACK_PX = 24;
const HISTOGRAM_BARS = 24;
const MIN_BAR_PERCENT = 4;
const CELL_RANGE_SECONDS = 60;
const RANGES = [{ seconds: 60, label: '1m' }, { seconds: 300, label: '5m' }, { seconds: 900, label: '15m' }];
const LEVELS: Level[] = ['INFO', 'WARN', 'ERROR'];
const CHART = { width: 600, height: 130, left: 30, right: 56, top: 12, bottom: 22 };
const CHART_TICKS = 5;
const CHART_HEADROOM = 1.2;
const MIN_CHART_RPS = 5;
const WARN_ERROR_RATE = 1;
const SHORT_TRACE_LENGTH = 8;
const COMPARE_SLOTS = 4;
const TRACES_URL = '/api/observability/traces';
const ENQUEUE_SPAN = 'queue.enqueue';
const WORKER_SPAN = 'worker.process';
const PURCHASED = 'purchased';
const MIN_SPAN_PERCENT = 0.6;
const STATUS_TONE: Record<TraceStatus, string> = { SUCCESS: 'ok', REJECTED: 'warn', PROCESSING: 'muted', QUEUED: 'muted', FAILED: 'bad' };
const STAGES = ['HTTP', 'CHANNEL', 'WORKER', 'DATABASE'];

const icons = {
  logs: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><rect x="5" y="3" width="14" height="18" rx="2"/><path d="M9 8h6M9 12h6M9 16h4"/></svg>',
  metrics: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linejoin="round" aria-hidden="true"><path d="M4 20h4v-7H4zM10 20h4V9h-4zM16 20h4V4h-4z"/></svg>',
  trace: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" aria-hidden="true"><path d="M3 5h9M7 10h8M10 15h11M5 20h6"/></svg>',
  pause: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true"><path class="pause-bars" d="M9 5v14M15 5v14"/><path class="play" d="M7 5l12 7-12 7z"/></svg>',
  clear: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M4 7h16M10 11v6M14 11v6M6 7l1 13h10l1-13M9 7V4h6v3"/></svg>',
};

const views = [
  { id: 'logs', label: 'LOGS', icon: icons.logs },
  { id: 'metrics', label: 'METRICS', icon: icons.metrics },
  { id: 'trace', label: 'TRACE', icon: icons.trace },
];

const markup = `
  <div class="mod-head">
    <span>09 / OBSERVABILITY</span>
    ${helpButtons('obs', 'Observability')}
  </div>
  <div class="mod-title"><span class="alg">Observability</span></div>
  <p class="race-subtitle open-only">Application logs, metrics and queue traces in real time.</p>
  <div class="obs-tiles cell-only" aria-hidden="true">${views.map((view) => `<span data-tile="${view.id}">${view.icon}${view.label}</span>`).join('')}</div>
  <div class="bars obs-histogram cell-only" aria-hidden="true">${'<span></span>'.repeat(HISTOGRAM_BARS)}</div>
  <div class="obs-tabs open-only" role="group" aria-label="View">
    ${views.map((view) => `<button type="button" data-view="${view.id}" aria-pressed="false">${view.icon}${view.label}</button>`).join('')}
  </div>
  <section class="obs-panel open-only" data-panel="logs">
    <div class="obs-panel-head">
      <div><h3>Application Logs</h3><p>Live stream · All modules · Structured logs</p></div>
      <div class="obs-stream"><span class="obs-chip" data-k="stream">● LIVE</span><span data-k="entries">0 entries</span></div>
    </div>
    <div class="obs-toolbar">
      <input type="search" data-f="search" placeholder="Search logs…" aria-label="Search logs">
      <select data-f="module" aria-label="Module"><option value="">All modules</option></select>
      <select data-f="level" aria-label="Level"><option value="">All levels</option>${LEVELS.map((level) => `<option>${level}</option>`).join('')}</select>
      <button type="button" class="obs-icon" data-a="pause" aria-pressed="false" aria-label="Pause stream" title="Pause stream">${icons.pause}</button>
      <button type="button" class="obs-icon" data-a="clear" aria-label="Clear logs" title="Clear logs">${icons.clear}</button>
    </div>
    <ol class="obs-log" role="log" aria-label="Application logs" tabindex="0"></ol>
  </section>
  <section class="obs-panel open-only" data-panel="metrics">
    <div class="obs-stats">
      <div><span>HTTP REQUESTS</span><b data-k="requests">–</b><small>total</small></div>
      <div><span>RPS</span><b data-k="rps">–</b><small>req/s</small></div>
      <div><span>AVG RESPONSE</span><b data-k="average">–</b><small>in range</small></div>
      <div><span>P95 LATENCY</span><b data-k="p95">–</b><small>in range</small></div>
      <div><span>HTTP 5XX RATE</span><b data-k="errors">–</b><small>in range</small></div>
    </div>
    <div class="obs-card">
      <div class="obs-card-head">
        <h3>Requests per second (RPS)</h3>
        <div class="obs-ranges" role="group" aria-label="Time range">
          ${RANGES.map((range) => `<button type="button" data-range="${range.seconds}" aria-pressed="false">${range.label}</button>`).join('')}
        </div>
      </div>
      <svg class="obs-chart" viewBox="0 0 ${CHART.width} ${CHART.height}" role="img" aria-label="Requests per second over the selected range">
        <g class="grid"></g>
        <path class="area"/>
        <path class="line"/>
        <circle class="tip" r="3.5"/>
        <text class="tip-label" text-anchor="end"></text>
        <g class="series">${Array.from({ length: COMPARE_SLOTS }, (_, slot) => `<path class="series-line series-${slot}"/>`).join('')}</g>
      </svg>
      <div class="obs-legend"></div>
    </div>
    <div class="obs-card">
      <div class="obs-card-head"><h3>Endpoint performance</h3><span class="obs-note" data-k="compare"></span></div>
      <div class="obs-table">
        <table>
          <thead><tr><th>ROUTE</th><th>METHOD</th><th>REQUESTS</th><th>RPS</th><th>P50</th><th>P95</th><th>ERROR RATE</th></tr></thead>
          <tbody></tbody>
        </table>
      </div>
    </div>
  </section>
  <section class="obs-panel obs-trace open-only" data-panel="trace">
    <div class="obs-panel-head">
      <div><h3>Queue trace</h3><p data-k="trace-note">Run a QUEUE race in 06 / RACE to record a trace.</p></div>
      <div class="obs-toolbar">
        <select data-f="run" aria-label="Race run"></select>
        <button type="button" class="obs-icon" data-a="trace-logs" aria-label="Open in Logs" title="Open in Logs">${icons.logs}</button>
        <button type="button" class="obs-icon" data-a="trace-reset" aria-label="Reset trace view" title="Reset trace view">↻</button>
      </div>
    </div>
    <div class="trace-requests" role="group" aria-label="Requests"></div>
    <div class="obs-stats">
      <div><span>REQUEST</span><b data-k="trace-request">–</b><small data-k="trace-id">–</small></div>
      <div><span>STATUS</span><b data-k="trace-status">–</b><small data-k="trace-outcome">–</small></div>
      <div><span>LIFECYCLE</span><b data-k="trace-lifecycle">–</b><small>enqueue → outcome</small></div>
      <div><span>QUEUE WAIT</span><b data-k="trace-wait">–</b><small>enqueue → dequeue</small></div>
      <div><span>WORKER</span><b data-k="trace-worker">–</b><small>processing</small></div>
    </div>
    <ol class="trace-flow" aria-label="Pipeline">${STAGES.map((stage) => `<li data-state="idle"><b>${stage}</b><small>–</small></li>`).join('')}</ol>
    <p class="trace-handoff" data-k="trace-handoff"></p>
    <div class="obs-card">
      <div class="obs-card-head"><h3>Timeline</h3><span class="obs-note" data-k="trace-total"></span></div>
      <ol class="trace-waterfall" aria-label="Spans of the selected request"></ol>
    </div>
    <div class="obs-card">
      <div class="obs-card-head"><h3>Correlated logs</h3><span class="obs-note" data-k="trace-log-count"></span></div>
      <ol class="obs-log trace-log" aria-label="Logs of the selected request"></ol>
    </div>
  </section>
  <div class="mod-status cell-only"><span class="status obs-live" role="status">● LIVE</span><span class="status-short" aria-hidden="true">● LIVE</span></div>
  ${helpPopovers('obs', '<p>A live look inside the API behind this page: the logs it writes, how fast every endpoint answers and a trace of each queued race. Use the other hexes and watch them show up here.</p>', `
    <p>Logs: every ILogger entry written by the API goes through a custom logger provider into a ${MAX_ROWS}-entry in-memory buffer and streams here over Server-Sent Events.</p>
    <p>Metrics: a MeterListener reads ASP.NET Core's own http.server.request.duration histogram and aggregates it per route template.</p>
    <p>Trace: an ActivityListener records the System.Diagnostics activities of QUEUE races. Each channel message carries its enqueue ActivityContext, so the worker span continues that trace and links back to the process request.</p>
    <p class="info-foot">Real requests from every hex · no Prometheus, no Grafana</p>`)}`;

const formatCount = (value: number) => value.toLocaleString('en-US');
const formatMs = (value?: number) => (value === undefined ? '–' : `${value < 10 ? value.toFixed(1) : Math.round(value)} ms`);
const formatPercent = (value?: number) => (value === undefined ? '–' : `${value.toFixed(1)} %`);
const formatClock = (date: Date) => date.toTimeString().slice(0, 8);
const formatTimestamp = (date: Date) => `${formatClock(date)}.${String(date.getMilliseconds()).padStart(3, '0')}`;

const formatSpanMs = (value?: number) => (value !== undefined && value < 1 ? `${value.toFixed(2)} ms` : formatMs(value));
const isSpan = (span?: TraceSpan): span is TraceSpan => span !== undefined;

function compareNote(count: number, full: boolean) {
  if (full) {
    return `max ${COMPARE_SLOTS} selected`;
  }

  if (count > 0) {
    return `${count} / ${COMPARE_SLOTS} on chart`;
  }

  return 'select rows to compare';
}

function barTiming(bar: Bar, origin: number, running: boolean) {
  if (bar.kind === 'outcome') {
    return `+${formatSpanMs(bar.start - origin)}`;
  }

  if (running) {
    return 'running';
  }

  return formatSpanMs(bar.durationMs);
}

const OUTCOME_TEXT: Record<TraceStatus, string> = {
  SUCCESS: 'purchased',
  REJECTED: 'out of stock · expected',
  PROCESSING: 'in the worker',
  QUEUED: 'in the channel',
  FAILED: 'processing error',
};

const keyOf = (endpoint: EndpointMetrics) => `${endpoint.method} ${endpoint.route}`;

function errorClass(rate?: number) {
  if (rate === undefined) {
    return 'muted';
  }

  if (rate === 0) {
    return 'ok';
  }

  return rate < WARN_ERROR_RATE ? 'warn' : 'bad';
}

function niceCeiling(value: number) {
  const magnitude = 10 ** Math.floor(Math.log10(value));

  return [1, 2, 5, 10].map((step) => step * magnitude).find((candidate) => candidate >= value)!;
}

function epochMs(iso: string) {
  const fraction = iso.match(/\.(\d+)/)?.[1] ?? '0';

  return Date.parse(iso.replace(/\.\d+/, '')) + Number(`0.${fraction}`) * 1000;
}

function journeyOf(spans: TraceSpan[], customer: number): Journey {
  const enqueue = spans.find((span) => span.name === ENQUEUE_SPAN && span.customer === customer);
  const worker = spans.find((span) => span.name === WORKER_SPAN && span.customer === customer);

  return {
    customer,
    request: spans.find((span) => span.spanId === enqueue?.parentSpanId),
    enqueue,
    processRequest: spans.find((span) => span.spanId === worker?.linkedSpanId),
    worker,
    database: worker ? spans.filter((span) => span.parentSpanId === worker.spanId) : [],
  };
}

function statusOf({ worker }: Journey): TraceStatus {
  if (!worker) {
    return 'QUEUED';
  }

  if (worker.failed) {
    return 'FAILED';
  }

  if (worker.durationMs === undefined) {
    return 'PROCESSING';
  }

  return worker.outcome === PURCHASED ? 'SUCCESS' : 'REJECTED';
}

function barsOf({ customer, request, enqueue, processRequest, worker, database }: Journey): Bar[] {
  const spanBar = (span: TraceSpan, label: string, depth: number): Bar => ({
    label, source: span.source, depth, start: epochMs(span.start), durationMs: span.durationMs, kind: 'span', failed: span.failed,
  });
  const bars: Bar[] = [];

  if (request) {
    bars.push(spanBar(request, request.name, 0));
  }

  if (enqueue) {
    bars.push(spanBar(enqueue, `enqueue #${customer}`, 1));
  }

  if (worker?.queueWaitMs !== undefined) {
    bars.push({ label: 'queue wait', source: 'QUEUE', depth: 1, start: epochMs(worker.start) - worker.queueWaitMs, durationMs: worker.queueWaitMs, kind: 'wait' });
  }

  if (processRequest) {
    bars.push(spanBar(processRequest, `${processRequest.name} · link`, 0));
  }

  if (worker) {
    bars.push(spanBar(worker, `dequeue + process #${customer}`, 1));
  }

  bars.push(...database.map((span) => spanBar(span, span.name, 2)));

  if (worker?.durationMs !== undefined && worker.outcome) {
    bars.push({ label: `outcome: ${worker.outcome.replaceAll('_', ' ')}`, source: 'WORKER', depth: 2, start: epochMs(worker.start) + worker.durationMs, kind: 'outcome', tone: worker.outcome === PURCHASED ? 'ok' : 'warn' });
  }

  return bars;
}

interface Stage {
  state: StageState;
  caption: string;
}

const IDLE_STAGE: Stage = { state: 'idle', caption: '–' };
const FAILED_STAGE: Stage = { state: 'failed', caption: 'error' };

function requestStage({ request }: Journey): Stage {
  if (request?.durationMs === undefined) {
    return { state: 'active', caption: 'handling' };
  }

  return { state: 'done', caption: `accepted · ${formatMs(request.durationMs)}` };
}

function queueStage({ enqueue, worker }: Journey): Stage {
  if (worker) {
    return { state: 'done', caption: `waited ${formatMs(worker.queueWaitMs)}` };
  }

  if (enqueue) {
    return { state: 'active', caption: 'waiting' };
  }

  return IDLE_STAGE;
}

function workerStage({ worker }: Journey): Stage {
  if (!worker) {
    return IDLE_STAGE;
  }

  if (worker.failed) {
    return FAILED_STAGE;
  }

  if (worker.durationMs === undefined) {
    return { state: 'active', caption: 'processing' };
  }

  return { state: 'done', caption: `sequential · ${formatMs(worker.durationMs)}` };
}

function databaseStage({ database }: Journey): Stage {
  if (database.length === 0) {
    return IDLE_STAGE;
  }

  if (database.some((span) => span.failed)) {
    return FAILED_STAGE;
  }

  if (database.some((span) => span.durationMs === undefined)) {
    return { state: 'active', caption: 'querying' };
  }

  const databaseMs = database.reduce((sum, span) => sum + (span.durationMs ?? 0), 0);
  const work = database.length === 1 ? 'stock check' : 'stock check + order';

  return { state: 'done', caption: `${work} · ${formatMs(databaseMs)}` };
}

function stagesOf(journey: Journey): Stage[] {
  return [requestStage(journey), queueStage(journey), workerStage(journey), databaseStage(journey)];
}

export const createObservabilityModule = () => {
  const element = document.createElement('section');
  element.className = 'mod mod-obs';
  element.setAttribute('aria-label', 'Observability');
  element.innerHTML = markup;

  const find = <T extends Element = Element>(selector: string) => element.querySelector<T>(selector)!;
  const put = (key: string, text: string) => {
    find(`[data-k="${key}"]`).textContent = text;
  };

  const status = find('.status');
  const statusShort = find('.status-short');
  const bars = [...element.querySelectorAll<HTMLElement>('.obs-histogram span')];
  const log = find<HTMLOListElement>('.obs-log');
  const search = find<HTMLInputElement>('[data-f=search]');
  const moduleFilter = find<HTMLSelectElement>('[data-f=module]');
  const levelFilter = find<HTMLSelectElement>('[data-f=level]');
  const pauseButton = find<HTMLButtonElement>('[data-a=pause]');
  const chart = find<SVGSVGElement>('.obs-chart');
  const tableBody = find<HTMLTableSectionElement>('.obs-table tbody');
  const legend = find<HTMLElement>('.obs-legend');
  const tableRows = new Map<string, HTMLTableRowElement>();
  const compared: (string | null)[] = Array(COMPARE_SLOTS).fill(null);
  let lastSnapshot: MetricsSnapshot | null = null;
  const totalLabel = document.createElement('span');
  totalLabel.className = 'legend-total';
  totalLabel.textContent = 'all traffic';

  let view = views[0].id;
  let range = RANGES[0].seconds;
  let timer = 0;
  let stream: EventSource | null = null;
  let paused = false;
  let following = true;
  let pending: LogEntry[] = [];
  let incoming: LogEntry[] = [];
  let hasOlder = false;
  let loadingOlder = false;
  let history = 0;
  let rows: { entry: LogEntry; row: HTMLLIElement }[] = [];
  const modules = new Set<string>();
  const tracePanel = find<HTMLElement>('.obs-trace');
  const runSelect = find<HTMLSelectElement>('[data-f=run]');
  const requestGroup = find<HTMLElement>('.trace-requests');
  const flowStages = [...element.querySelectorAll<HTMLLIElement>('.trace-flow li')];
  const waterfall = find<HTMLOListElement>('.trace-waterfall');
  const traceLog = find<HTMLOListElement>('.trace-log');
  let pinnedRun: string | null = null;
  let customer = 0;
  let trace: TraceDetail | null = null;
  let drawnTrace = '';
  let listedRuns = '';

  const isOpen = () => 'open' in element.dataset;

  function setStatus(failed: boolean) {
    status.textContent = failed ? '✕ API UNREACHABLE' : '● LIVE';
    statusShort.textContent = failed ? '✕ OFFLINE' : '● LIVE';

    if (failed) {
      element.dataset.result = 'failed';
      return;
    }

    delete element.dataset.result;
  }

  function selectView(next: string) {
    view = next;
    element.querySelectorAll<HTMLButtonElement>('[data-view]').forEach((button) => button.setAttribute('aria-pressed', String(button.dataset.view === next)));
    element.querySelectorAll<HTMLElement>('[data-panel]').forEach((panel) => {
      panel.hidden = panel.dataset.panel !== next;
    });
    element.querySelectorAll<HTMLElement>('[data-tile]').forEach((tile) => tile.classList.toggle('selected', tile.dataset.tile === next));
    scrollToNewest();
    loadOlderIfNear();

    if (next === 'trace' && timer) {
      loadTrace();
    }
  }

  function matches(entry: LogEntry) {
    const query = search.value.trim().toLowerCase();

    if (levelFilter.value && entry.level !== levelFilter.value) {
      return false;
    }

    if (moduleFilter.value && entry.module !== moduleFilter.value) {
      return false;
    }

    return !query || `${entry.module} ${entry.message} ${entry.traceId ?? ''}`.toLowerCase().includes(query);
  }

  function applyFilters() {
    rows.forEach(({ entry, row }) => {
      row.hidden = !matches(entry);
    });
    scrollToNewest();
    loadOlderIfNear();
  }

  function scrollToNewest() {
    if (following) {
      log.scrollTop = 0;
    }
  }

  function addModule(name: string) {
    if (modules.has(name)) {
      return;
    }

    modules.add(name);
    const option = document.createElement('option');
    option.textContent = name;
    const after = [...moduleFilter.options].slice(1).find((existing) => existing.value.localeCompare(name) > 0);
    moduleFilter.insertBefore(option, after ?? null);
  }

  function renderRow(entry: LogEntry, source = entry.module) {
    const row = document.createElement('li');
    row.dataset.level = entry.level;
    const parts = [
      { className: 'time', text: formatTimestamp(new Date(entry.timestamp)) },
      { className: 'level', text: entry.level },
      { className: 'source', text: `[${source}]` },
      { className: 'message', text: entry.message },
    ];
    row.append(...parts.map(({ className, text }) => {
      const part = document.createElement('span');
      part.className = className;
      part.textContent = text;

      return part;
    }));
    if (entry.traceId) {
      const trace = document.createElement('span');
      trace.className = 'trace';
      trace.textContent = ` trace=${entry.traceId.slice(0, SHORT_TRACE_LENGTH)}`;
      trace.title = `traceId ${entry.traceId} · spanId ${entry.spanId}`;
      row.querySelector('.message')!.append(trace);
    }

    return row;
  }

  function showEntries(entries: LogEntry[]) {
    entries.forEach((entry) => addModule(entry.module));
    const added = entries.map((entry) => ({ entry, row: renderRow(entry) }));
    added.forEach(({ entry, row }) => {
      row.hidden = !matches(entry);
    });
    const heightBefore = log.scrollHeight;
    log.prepend(...added.map(({ row }) => row).reverse());

    if (following) {
      log.scrollTop = 0;
    } else {
      log.scrollTop += log.scrollHeight - heightBefore;
    }

    rows.push(...added);
    const trimmed = rows.splice(0, Math.max(0, rows.length - MAX_ROWS));
    trimmed.forEach(({ row }) => row.remove());
    hasOlder ||= trimmed.length > 0;
    put('entries', `${formatCount(rows.length)} entries`);
  }

  function showOlder(entries: LogEntry[]) {
    entries.forEach((entry) => addModule(entry.module));
    const added = entries.map((entry) => ({ entry, row: renderRow(entry) }));
    added.forEach(({ entry, row }) => {
      row.hidden = !matches(entry);
    });
    log.append(...added.map(({ row }) => row));
    rows.unshift(...added.reverse());
    put('entries', `${formatCount(rows.length)} entries`);
  }

  async function loadOlder() {
    if (loadingOlder || !hasOlder) {
      return;
    }

    loadingOlder = true;
    const requested = history;
    const before = rows.length ? `&before=${rows[0].entry.id}` : '';
    const entries: LogEntry[] | null = await api(`${LOG_HISTORY_URL}?size=${LOG_PAGE_SIZE}${before}`, AbortSignal.timeout(POLL_MS)).catch(() => null);
    loadingOlder = false;

    if (requested !== history) {
      return;
    }

    if (!entries) {
      put('stream', '✕ HISTORY UNAVAILABLE');
      return;
    }

    hasOlder = entries.length === LOG_PAGE_SIZE;
    showOlder(entries);
    loadOlderIfNear();
  }

  function loadOlderIfNear() {
    if (log.clientHeight && log.scrollTop + log.clientHeight >= log.scrollHeight - OLDER_SLACK_PX) {
      loadOlder();
    }
  }

  function flushIncoming() {
    showEntries(incoming);
    incoming = [];
  }

  function receive(entry: LogEntry) {
    if (!paused) {
      if (!incoming.length) {
        requestAnimationFrame(flushIncoming);
      }

      incoming.push(entry);
      return;
    }

    pending.push(entry);
    pending.splice(0, Math.max(0, pending.length - MAX_ROWS));
    put('stream', `❚❚ PAUSED · ${pending.length} new`);
  }

  function clearLogs() {
    rows.forEach(({ row }) => row.remove());
    rows = [];
    pending = [];
    incoming = [];
    hasOlder = false;
    history++;
    following = true;
    put('entries', '0 entries');
    showStreamState();
  }

  function showStreamState() {
    if (paused) {
      put('stream', `❚❚ PAUSED · ${pending.length} new`);
      return;
    }

    put('stream', stream?.readyState === EventSource.OPEN ? '● LIVE' : '○ CONNECTING');
  }

  function setPaused(next: boolean) {
    paused = next;
    pauseButton.setAttribute('aria-pressed', String(next));
    pauseButton.setAttribute('aria-label', next ? 'Resume stream' : 'Pause stream');
    pauseButton.title = next ? 'Resume stream' : 'Pause stream';

    if (!next) {
      showEntries(pending);
      pending = [];
    }

    showStreamState();
  }

  function connect() {
    if (stream) {
      return;
    }

    stream = new EventSource(LOGS_URL);
    stream.addEventListener('open', showStreamState);
    stream.addEventListener('ready', () => {
      clearLogs();
      hasOlder = true;
      loadOlder();
    });
    stream.addEventListener('log', (event) => receive(JSON.parse((event as MessageEvent<string>).data)));
    stream.addEventListener('error', () => {
      if (stream?.readyState === EventSource.CLOSED) {
        disconnect();
        put('stream', '✕ OFFLINE');
        return;
      }

      showStreamState();
    });
    showStreamState();
    element.closest('dialog')?.addEventListener('close', disconnect, { once: true });
  }

  function disconnect() {
    stream?.close();
    stream = null;
  }

  function setPath(selector: string, path: string) {
    const target = chart.querySelector<SVGPathElement>(selector)!;
    target.setAttribute('d', path);
    target.style.setProperty('d', `path("${path}")`);
  }

  function drawGrid(top: number) {
    const plotHeight = CHART.height - CHART.top - CHART.bottom;
    const plotWidth = CHART.width - CHART.left - CHART.right;
    const now = Date.now();
    const yTicks = Array.from({ length: CHART_TICKS + 1 }, (_, index) => {
      const y = CHART.top + plotHeight - (index / CHART_TICKS) * plotHeight;

      return `<line x1="${CHART.left}" x2="${CHART.width - CHART.right}" y1="${y}" y2="${y}"/><text x="${CHART.left - 6}" y="${y + 3}" text-anchor="end">${(top * index) / CHART_TICKS}</text>`;
    });
    const xTicks = Array.from({ length: CHART_TICKS + 1 }, (_, index) => {
      const x = CHART.left + (index / CHART_TICKS) * plotWidth;
      const time = new Date(now - range * 1000 * (1 - index / CHART_TICKS));

      return `<text x="${x}" y="${CHART.height - 6}" text-anchor="middle">${formatClock(time)}</text>`;
    });
    find('.obs-chart .grid').innerHTML = yTicks.join('') + xTicks.join('');
  }

  function pointsOf(series: number[], top: number) {
    const plotHeight = CHART.height - CHART.top - CHART.bottom;
    const plotWidth = CHART.width - CHART.left - CHART.right;

    return series.map((value, index) => ({
      x: CHART.left + (index / (series.length - 1)) * plotWidth,
      y: CHART.top + plotHeight - (value / top) * plotHeight,
    }));
  }

  const lineThrough = (points: { x: number; y: number }[]) => points.map((point, index) => `${index === 0 ? 'M' : 'L'}${point.x.toFixed(1)} ${point.y.toFixed(1)}`).join(' ');

  function drawChart({ rps: series, endpoints }: MetricsSnapshot) {
    const top = niceCeiling(Math.max(MIN_CHART_RPS, ...series) * CHART_HEADROOM);
    const points = pointsOf(series, top);
    const line = lineThrough(points);
    const baseline = CHART.height - CHART.bottom;
    const area = `${line} L${points[points.length - 1].x} ${baseline} L${points[0].x} ${baseline} Z`;
    const last = points[points.length - 1];

    drawGrid(top);
    setPath('.line', line);
    setPath('.area', area);
    compared.forEach((key, slot) => {
      const endpoint = endpoints.find((candidate) => keyOf(candidate) === key);
      setPath(`.series-${slot}`, key ? lineThrough(pointsOf(endpoint?.rpsSeries ?? series.map(() => 0), top)) : '');
    });
    chart.querySelector('.tip')!.setAttribute('cx', String(last.x));
    chart.querySelector('.tip')!.setAttribute('cy', String(last.y));
    const label = chart.querySelector('.tip-label')!;
    label.setAttribute('x', String(CHART.width - 2));
    label.setAttribute('y', String(Math.max(CHART.top + 8, last.y - 8)));
    label.textContent = `${series[series.length - 1].toFixed(1)} rps`;
  }

  function createRow(endpoint: EndpointMetrics) {
    const row = document.createElement('tr');
    row.dataset.key = keyOf(endpoint);
    row.innerHTML = '<td><button type="button" class="route-toggle" aria-pressed="false"><span class="dot"></span><span class="route"></span></button></td><td><span class="method"></span></td><td></td><td></td><td></td><td></td><td><span></span></td>';
    row.querySelector('.route')!.textContent = endpoint.route;
    const method = row.querySelector('.method')!;
    method.textContent = endpoint.method;
    method.classList.add(`method-${endpoint.method.toLowerCase()}`);

    return row;
  }

  function fillRow(row: HTMLTableRowElement, endpoint: EndpointMetrics) {
    const cells = row.cells;
    cells[2].textContent = formatCount(endpoint.requests);
    cells[3].textContent = endpoint.rps.toFixed(1);
    cells[4].textContent = formatMs(endpoint.p50Ms);
    cells[5].textContent = formatMs(endpoint.p95Ms);
    const errors = cells[6].firstElementChild!;
    errors.textContent = formatPercent(endpoint.errorRate);
    errors.className = errorClass(endpoint.errorRate);
  }

  function drawTable(endpoints: EndpointMetrics[]) {
    const ordered = endpoints.map((endpoint) => {
      const key = keyOf(endpoint);
      const row = tableRows.get(key) ?? createRow(endpoint);
      tableRows.set(key, row);
      fillRow(row, endpoint);

      return row;
    });

    if (ordered.some((row, index) => tableBody.children[index] !== row)) {
      tableBody.replaceChildren(...ordered);
    }
  }

  function paintComparison() {
    const full = compared.every((key) => key !== null);
    const count = compared.filter((key) => key !== null).length;
    tableRows.forEach((row, key) => {
      const slot = compared.indexOf(key);
      const toggle = row.querySelector('.route-toggle')!;
      toggle.setAttribute('aria-pressed', String(slot >= 0));
      toggle.setAttribute('aria-disabled', String(slot < 0 && full));
      row.querySelector('.dot')!.className = slot >= 0 ? `dot series-${slot}` : 'dot';
    });
    chart.toggleAttribute('data-compare', count > 0);
    put('compare', compareNote(count, full));
    const chips = compared.flatMap((key, slot) => {
      if (!key) {
        return [];
      }

      const chip = document.createElement('button');
      chip.type = 'button';
      chip.className = `chip series-${slot}`;
      chip.dataset.key = key;
      chip.setAttribute('aria-label', `Remove ${key} from chart`);
      chip.innerHTML = '<span class="dot"></span><span></span> ✕';
      chip.children[1].textContent = key;

      return [chip];
    });
    legend.replaceChildren(...(count ? [totalLabel, ...chips] : []));

    if (lastSnapshot) {
      drawChart(lastSnapshot);
    }
  }

  function toggleCompared(key: string) {
    const slot = compared.indexOf(key);

    if (slot >= 0) {
      compared[slot] = null;
      paintComparison();
      return;
    }

    const free = compared.indexOf(null);

    if (free < 0) {
      return;
    }

    compared[free] = key;
    paintComparison();
  }

  function drawHistogram(series: number[]) {
    const recent = series.slice(-HISTOGRAM_BARS);
    const highest = Math.max(1, ...recent);
    bars.forEach((bar, index) => {
      bar.style.height = `${Math.max(MIN_BAR_PERCENT, (100 * (recent[index] ?? 0)) / highest)}%`;
    });
  }

  function showMetrics(snapshot: MetricsSnapshot) {
    const { overview } = snapshot;
    put('requests', formatCount(overview.requests));
    put('rps', overview.rps.toFixed(1));
    put('average', formatMs(overview.averageMs));
    put('p95', formatMs(overview.p95Ms));
    put('errors', formatPercent(overview.errorRate));
    lastSnapshot = snapshot;
    drawTable(snapshot.endpoints);
    paintComparison();
  }

  function fillRuns(runs: TraceRun[]) {
    const options = [
      new Option('Latest run', ''),
      ...runs.map((run) => new Option(`${formatTimestamp(new Date(run.startedAt))} · ${run.finished}/${run.requests} processed`, run.runId)),
    ];
    const listed = options.map((option) => `${option.value} ${option.text}`).join('|');

    if (listed !== listedRuns) {
      listedRuns = listed;
      runSelect.replaceChildren(...options);
    }

    runSelect.value = pinnedRun ?? '';
  }

  function drawRequests(spans: TraceSpan[], customers: number[]) {
    if (requestGroup.children.length !== customers.length) {
      requestGroup.replaceChildren(...customers.map(() => {
        const button = document.createElement('button');
        button.type = 'button';

        return button;
      }));
    }

    [...requestGroup.querySelectorAll('button')].forEach((button, index) => {
      const number = customers[index];
      const status = statusOf(journeyOf(spans, number));
      button.dataset.customer = String(number);
      button.textContent = String(number).padStart(2, '0');
      button.className = STATUS_TONE[status];
      button.title = status;
      button.setAttribute('aria-label', `Request ${number}: ${status}`);
      button.setAttribute('aria-pressed', String(number === customer));
    });
  }

  function drawSummary(journey: Journey) {
    const { enqueue, worker } = journey;
    const status = statusOf(journey);
    const finished = enqueue && worker?.durationMs !== undefined;
    put('trace-request', `#${String(journey.customer).padStart(2, '0')}`);
    put('trace-id', enqueue ? `trace ${enqueue.traceId.slice(0, SHORT_TRACE_LENGTH)}` : '–');
    find<HTMLElement>('[data-k="trace-id"]').title = enqueue ? `traceId ${enqueue.traceId} · spanId ${enqueue.spanId}` : '';
    put('trace-status', status);
    find('[data-k="trace-status"]').className = STATUS_TONE[status];
    put('trace-outcome', OUTCOME_TEXT[status]);
    put('trace-lifecycle', finished ? formatSpanMs(epochMs(worker.start) + worker.durationMs! - epochMs(enqueue.start)) : '–');
    put('trace-wait', formatSpanMs(worker?.queueWaitMs));
    put('trace-worker', formatSpanMs(worker?.durationMs));
  }

  function drawFlow(journey: Journey) {
    stagesOf(journey).forEach((stage, index) => {
      flowStages[index].dataset.state = stage.state;
      flowStages[index].querySelector('small')!.textContent = stage.caption;
    });

    const { request, worker } = journey;

    if (!request) {
      put('trace-handoff', 'Async handoff: POST /api/race/queue still running.');
      return;
    }

    const decided = worker?.durationMs === undefined
      ? 'business outcome pending'
      : `worker decided at +${formatSpanMs(epochMs(worker.start) + worker.durationMs - epochMs(request.start))}`;
    put('trace-handoff', `Async handoff: HTTP accepted at +${formatSpanMs(request.durationMs)} · ${decided}`);
  }

  function drawWaterfall(journey: Journey) {
    const bars = barsOf(journey);
    const origin = Math.min(...bars.map((bar) => bar.start));
    const end = Math.max(...bars.map((bar) => bar.start + (bar.durationMs ?? 0)));
    const total = Math.max(end - origin, Number.EPSILON);
    put('trace-total', `${formatSpanMs(end - origin)} shown`);
    waterfall.replaceChildren(...bars.map((bar) => {
      const row = document.createElement('li');
      const left = ((bar.start - origin) / total) * 100;
      const running = bar.kind === 'span' && bar.durationMs === undefined;
      const width = bar.kind === 'outcome' || running ? 100 - left : Math.max(MIN_SPAN_PERCENT, ((bar.durationMs ?? 0) / total) * 100);
      row.dataset.source = bar.source;
      row.dataset.kind = running ? 'running' : bar.kind;
      row.toggleAttribute('data-failed', bar.failed === true);

      if (bar.tone) {
        row.classList.add(bar.tone);
      }

      row.style.setProperty('--depth', String(bar.depth));
      row.title = `${bar.source} · starts at +${formatSpanMs(bar.start - origin)}`;
      row.innerHTML = '<span class="name"></span><span class="track"><span class="bar"></span></span><span class="ms"></span>';
      row.querySelector('.name')!.textContent = bar.label;
      row.querySelector<HTMLElement>('.bar')!.style.cssText = `left: ${left}%; width: ${Math.min(width, 100 - left)}%`;
      row.querySelector('.ms')!.textContent = barTiming(bar, origin, running);

      return row;
    }));
  }

  function drawTraceLogs(logs: LogEntry[], { request, enqueue, processRequest, worker, database }: Journey) {
    const sources = new Map([request, enqueue, processRequest, worker, ...database].filter(isSpan).map((span) => [span.spanId, span.source]));
    const entries = logs.filter((entry) => entry.spanId && sources.has(entry.spanId));
    traceLog.replaceChildren(...entries.map((entry) => renderRow(entry, sources.get(entry.spanId!))));
    put('trace-log-count', `${entries.length} entries`);
  }

  function drawTrace(detail: TraceDetail | null) {
    const signature = `${JSON.stringify(detail)} ${customer}`;

    if (signature === drawnTrace) {
      return;
    }

    drawnTrace = signature;
    trace = detail;
    const enqueues = detail?.spans.filter((span) => span.name === ENQUEUE_SPAN) ?? [];
    const customers = enqueues.map((span) => span.customer!).sort((first, second) => first - second);
    tracePanel.toggleAttribute('data-empty', customers.length === 0);

    if (!detail || customers.length === 0) {
      put('trace-note', 'Run a QUEUE race in 06 / RACE to record a trace.');
      requestGroup.replaceChildren();
      return;
    }

    if (!customers.includes(customer)) {
      customer = customers[0];
    }

    const finished = detail.spans.filter((span) => span.name === WORKER_SPAN && span.durationMs !== undefined).length;
    put('trace-note', `Run ${detail.runId.slice(0, SHORT_TRACE_LENGTH)} · ${finished}/${customers.length} processed · read-only, races start in 06 / RACE`);
    const journey = journeyOf(detail.spans, customer);
    drawRequests(detail.spans, customers);
    drawSummary(journey);
    drawFlow(journey);
    drawWaterfall(journey);
    drawTraceLogs(detail.logs, journey);
  }

  async function refreshTrace() {
    const runs: TraceRun[] = await api(TRACES_URL, AbortSignal.timeout(POLL_MS));

    if (pinnedRun && !runs.some((run) => run.runId === pinnedRun)) {
      pinnedRun = null;
    }

    fillRuns(runs);
    const runId = pinnedRun ?? runs[0]?.runId;
    drawTrace(runId ? await api(`${TRACES_URL}/${runId}`, AbortSignal.timeout(POLL_MS)) : null);
  }

  function loadTrace() {
    refreshTrace().catch(() => setStatus(true));
  }

  function resetTrace() {
    pinnedRun = null;
    customer = 0;
    drawnTrace = '';
    loadTrace();
  }

  function openTraceLogs() {
    const traceId = trace && journeyOf(trace.spans, customer).enqueue?.traceId;

    if (!traceId) {
      return;
    }

    search.value = traceId;
    moduleFilter.value = '';
    levelFilter.value = '';
    following = true;
    applyFilters();
    selectView('logs');
  }

  async function refresh() {
    const seconds = isOpen() ? range : CELL_RANGE_SECONDS;

    try {
      const snapshot: MetricsSnapshot = await api(`${METRICS_URL}?seconds=${seconds}`, AbortSignal.timeout(POLL_MS));
      setStatus(false);

      if (isOpen()) {
        showMetrics(snapshot);

        if (view === 'trace') {
          await refreshTrace();
        }

        return;
      }

      drawHistogram(snapshot.rps);
    } catch {
      setStatus(true);
    }
  }

  function selectRange(seconds: number) {
    range = seconds;
    element.querySelectorAll<HTMLButtonElement>('[data-range]').forEach((button) => button.setAttribute('aria-pressed', String(Number(button.dataset.range) === seconds)));

    if (timer) {
      refresh();
    }
  }

  element.querySelectorAll<HTMLButtonElement>('[data-view]').forEach((button) => button.addEventListener('click', () => selectView(button.dataset.view!)));
  element.querySelectorAll<HTMLButtonElement>('[data-range]').forEach((button) => button.addEventListener('click', () => selectRange(Number(button.dataset.range))));
  [search, moduleFilter, levelFilter].forEach((filter) => filter.addEventListener('input', applyFilters));
  pauseButton.addEventListener('click', () => setPaused(!paused));
  tableBody.addEventListener('click', (event) => {
    const key = (event.target as Element).closest<HTMLElement>('tr')?.dataset.key;

    if (key) {
      toggleCompared(key);
    }
  });
  legend.addEventListener('click', (event) => {
    const key = (event.target as Element).closest<HTMLElement>('.chip')?.dataset.key;

    if (key) {
      toggleCompared(key);
    }
  });
  find('[data-a=clear]').addEventListener('click', clearLogs);
  find('[data-a=trace-reset]').addEventListener('click', resetTrace);
  find('[data-a=trace-logs]').addEventListener('click', openTraceLogs);
  runSelect.addEventListener('change', () => {
    pinnedRun = runSelect.value || null;
    loadTrace();
  });
  requestGroup.addEventListener('click', (event) => {
    const number = (event.target as Element).closest<HTMLElement>('[data-customer]')?.dataset.customer;

    if (number !== undefined && trace) {
      customer = Number(number);
      drawTrace(trace);
    }
  });
  log.addEventListener('scroll', () => {
    following = log.scrollTop <= FOLLOW_SLACK_PX;
    loadOlderIfNear();
  }, { passive: true });

  selectView(view);
  selectRange(range);
  drawHistogram([]);
  paintComparison();

  return {
    element,
    showTrace(runId: string) {
      pinnedRun = runId;
      drawnTrace = '';
      selectView('trace');
    },
    setActive(active: boolean) {
      if (!active) {
        clearInterval(timer);
        timer = 0;
        disconnect();
        return;
      }

      if (isOpen()) {
        connect();
      }

      if (!timer) {
        timer = setInterval(refresh, POLL_MS);
      }

      refresh();
    },
  };
};
