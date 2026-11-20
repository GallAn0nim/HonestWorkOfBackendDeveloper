import './style.css';
import { createSortingEngine } from './sortingModule';
import { createMaze } from './mazeModule';
import { createLoginModule } from './loginModule';
import { createCacheModule } from './cacheModule';
import { createRaceModule } from './raceModule';
import { createObservabilityModule } from './observabilityModule';
import { createQuoteModule } from './quoteModule';

interface Module {
  element: HTMLElement;
  setActive(active: boolean): void;
}

let signedIn = false;
const login = createLoginModule((session) => {
  signedIn = session;

  if (lit) {
    setModulesActive(true);
  }

  if (!dialog.open) {
    applyAccess();
    return;
  }

  dialog.close();

  if (session) {
    revealModules(applyAccess());
    return;
  }

  lockModules();
});

const observability = createObservabilityModule();
const race = createRaceModule((runId) => {
  observability.showTrace(runId);
  dialog.addEventListener('close', () => openHex(observability, hexes[slots.indexOf(observability)]), { once: true });
  dialog.close();
});

const slots: (Module | null)[] = [null, createMaze(), createSortingEngine(), createCacheModule(), login, race, null, createQuoteModule(), observability];

const isAvailable = (index: number) => slots[index] !== null && (signedIn || slots[index] === login);
const COLUMNS = 3;
const HEX_HEIGHT_PER_WIDTH = 200 / 174;
const PHONE_GUTTER = 16;

const SIGNAL_MS = 1000;
const TRAIL_MS = 450;
const FADE_MS = 1000;
const LOCK_FADE_MS = 500;
const FAR_FADE = 0.75;
const HEX_VERTICES = [
  { x: 0.5, y: 0 },
  { x: 1, y: 0.25 },
  { x: 1, y: 0.75 },
  { x: 0.5, y: 1 },
  { x: 0, y: 0.75 },
  { x: 0, y: 0.25 },
];
const MERGE_DISTANCE = 20;
const SPARK_SEGMENTS = 4;
const SPARK_JITTER = 3;
const OUTER_CORNER_LINKS = 2;
const SVG_NS = 'http://www.w3.org/2000/svg';
const reducedMotion = matchMedia('(prefers-reduced-motion: reduce)');
const TOUCH_LIFT = 80;
const KEY_STEP = 24;
const TAP_SLOP = 10;
const LENS_RADIUS_PER_HEX_HEIGHT = 0.75;
const FIT_MARGIN = 0.96;
const BULB_CLICKS = 50;
const BULB_VISIBLE_MS = 5 * 60_000;
const BULB_TICK_MS = 1000;
const LIGHT_FADE_MS = 600;
const DARK_VEIL = 0.98;
const LIT_VEIL = 0;
const EDGE_GLOWS = [
  { width: 1.5, blur: 0, alpha: 0.8 },
  { width: 3, blur: 6, alpha: 0.6 },
  { width: 6, blur: 24, alpha: 0.25 },
];
const LIGHT_COLOR = '178, 241, 140';
const HINT_MIN_MS = 3000;
const HINT_MAX_MS = 6000;
const HINT_GLOW_MS = 1400;
const ARROW_DIRECTIONS: Record<string, { x: number; y: number }> = {
  ArrowLeft: { x: -1, y: 0 },
  ArrowRight: { x: 1, y: 0 },
  ArrowUp: { x: 0, y: -1 },
  ArrowDown: { x: 0, y: 1 },
};

const world = document.querySelector<HTMLElement>('.world')!;
const header = document.querySelector<HTMLElement>('header')!;
const comb = document.querySelector<HTMLElement>('.comb')!;
const ring = document.querySelector<HTMLElement>('.lens-ring')!;
const toggle = document.querySelector<HTMLButtonElement>('.inspect-btn')!;
const toggleLabel = document.querySelector<HTMLElement>('.inspect-label')!;
const dialog = document.querySelector<HTMLDialogElement>('.open-hex')!;
const bigHex = dialog.querySelector<HTMLElement>('.hex-inner')!;

let statusIds = 0;
const hexes = slots.map((module, index) => {
  const hex = document.createElement('div');
  hex.className = 'hex';
  const inner = document.createElement('div');
  inner.className = 'hex-inner';
  hex.append(inner);
  hex.hidden = !module;

  if (module) {
    const status = module.element.querySelector('[role=status]');

    if (status) {
      status.id ||= `status-${statusIds++}`;
    }

    inner.append(module.element);
    hex.addEventListener('click', () => {
      if (!dragged && isAvailable(index)) {
        openHex(module, hex);
      }
    });
    hex.addEventListener('keydown', (event) => {
      if (isAvailable(index) && (event.key === 'Enter' || event.key === ' ')) {
        event.preventDefault();
        openHex(module, hex);
      }
    });
  }

  comb.append(hex);

  return hex;
});

function applyAccess() {
  const unlocked: HTMLElement[] = [];

  slots.forEach((module, index) => {
    if (!module) {
      return;
    }

    const hex = hexes[index];
    const available = isAvailable(index);

    if (available && !hex.classList.contains('module')) {
      unlocked.push(module.element);
    }

    module.element.hidden = !available;
    hex.classList.toggle('module', available);

    if (!available) {
      hex.removeAttribute('tabindex');
      hex.removeAttribute('role');
      hex.removeAttribute('aria-label');
      hex.removeAttribute('aria-describedby');
      return;
    }

    hex.tabIndex = 0;
    hex.setAttribute('role', 'button');
    hex.setAttribute('aria-label', `Open ${module.element.getAttribute('aria-label')}`);
    const status = module.element.querySelector('[role=status]');

    if (status) {
      hex.setAttribute('aria-describedby', status.id);
    }
  });

  if (activeIndex >= 0 && !isAvailable(activeIndex)) {
    setActiveIndex(-1);
  }

  if (inspecting && !dialog.open) {
    moveLens(lens.x, lens.y);
  }

  if (lit) {
    setModulesActive(true);
  }

  return unlocked;
}

let holdingLens = false;

interface Point {
  x: number;
  y: number;
}

function vertexOf(index: number, vertex: number): Point {
  const hex = hexes[index];
  const corner = HEX_VERTICES[vertex];

  return { x: hex.offsetLeft + corner.x * hex.offsetWidth, y: hex.offsetTop + corner.y * hex.offsetHeight };
}

function honeycomb() {
  const groups: Point[][] = [];
  const links: Set<number>[] = [];

  const groupOf = (point: Point) => {
    const found = groups.findIndex((group) => Math.hypot(group[0].x - point.x, group[0].y - point.y) < MERGE_DISTANCE);

    if (found >= 0) {
      groups[found].push(point);
      return found;
    }

    groups.push([point]);
    links.push(new Set());

    return groups.length - 1;
  };

  const corners = hexes.map((hex, index) => (hex.hidden ? [] : HEX_VERTICES.map((_, vertex) => groupOf(vertexOf(index, vertex)))));
  corners.forEach((ids) => ids.forEach((id, vertex) => {
    const next = ids[(vertex + 1) % ids.length];
    links[id].add(next);
    links[next].add(id);
  }));
  const points = groups.map((group) => ({
    x: group.reduce((sum, point) => sum + point.x, 0) / group.length,
    y: group.reduce((sum, point) => sum + point.y, 0) / group.length,
  }));

  return { points, links, starts: corners[slots.indexOf(login)] };
}

function sparkTree(points: Point[], links: Set<number>[], starts: number[]) {
  const arrival = new Map<number, number>();
  const parent = new Map<number, number>();
  const reached = new Set<number>();
  starts.forEach((id) => arrival.set(id, 0));

  while (reached.size < arrival.size) {
    const pending = [...arrival.keys()].filter((id) => !reached.has(id));
    const current = pending.reduce((earliest, id) => (arrival.get(id)! < arrival.get(earliest)! ? id : earliest));
    const time = arrival.get(current)!;
    reached.add(current);

    for (const next of links[current]) {
      const length = Math.hypot(points[next].x - points[current].x, points[next].y - points[current].y);
      const reach = time + length * (0.4 + Math.random() * 1.2);

      if (!reached.has(next) && reach < (arrival.get(next) ?? Infinity)) {
        arrival.set(next, reach);
        parent.set(next, current);
      }
    }
  }

  return { arrival, parent };
}

function drawSpark(layer: SVGSVGElement, from: Point, to: Point, delay: number, duration: number, strength: number) {
  const length = Math.hypot(to.x - from.x, to.y - from.y);
  const normal = { x: (from.y - to.y) / length, y: (to.x - from.x) / length };
  const zigzag = Array.from({ length: SPARK_SEGMENTS + 1 }, (_, step) => {
    const along = step / SPARK_SEGMENTS;
    const offset = step === 0 || step === SPARK_SEGMENTS ? 0 : (Math.random() * 2 - 1) * SPARK_JITTER;

    return `${from.x + (to.x - from.x) * along + normal.x * offset},${from.y + (to.y - from.y) * along + normal.y * offset}`;
  });
  const spark = document.createElementNS(SVG_NS, 'polyline');
  spark.setAttribute('points', zigzag.join(' '));
  spark.setAttribute('pathLength', '1');
  spark.classList.add('spark');
  layer.append(spark);
  const total = duration + TRAIL_MS;
  const drawn = duration / total;

  return spark.animate([
    { strokeDashoffset: 1, opacity: strength },
    { strokeDashoffset: 0, opacity: strength, offset: drawn },
    { strokeDashoffset: 0, opacity: 0.3 * strength, offset: drawn + (1 - drawn) * 0.3 },
    { strokeDashoffset: 0, opacity: 0.8 * strength, offset: drawn + (1 - drawn) * 0.5 },
    { strokeDashoffset: 0, opacity: 0 },
  ], { duration: total, delay }).finished;
}

function flashCorner(layer: SVGSVGElement, corner: Point, delay: number, strength: number) {
  const flash = document.createElementNS(SVG_NS, 'circle');
  flash.setAttribute('cx', String(corner.x));
  flash.setAttribute('cy', String(corner.y));
  flash.setAttribute('r', '3.5');
  flash.classList.add('particle');
  flash.style.opacity = '0';
  layer.append(flash);

  return flash.animate([{ opacity: 0 }, { opacity: strength, offset: 0.15 }, { opacity: 0 }], { duration: TRAIL_MS, delay }).finished;
}

function sendSignals(inward: boolean) {
  const { points, links, starts } = honeycomb();
  const { arrival, parent } = sparkTree(points, links, starts);
  const scale = SIGNAL_MS / Math.max(...arrival.values());
  const at = (id: number) => (inward ? SIGNAL_MS - arrival.get(id)! * scale : arrival.get(id)! * scale);
  const centre = centres[slots.indexOf(login)];
  const farthest = Math.max(...points.map((point) => Math.hypot(point.x - centre.x, point.y - centre.y)));
  const strength = (from: Point, to: Point) => 1 - (FAR_FADE * Math.hypot((from.x + to.x) / 2 - centre.x, (from.y + to.y) / 2 - centre.y)) / farthest;
  const layer = document.createElementNS(SVG_NS, 'svg');
  layer.classList.add('signals');
  comb.append(layer);

  const sparks = [...parent.keys()].map((id) => {
    const towardCentre = parent.get(id)!;
    const from = inward ? id : towardCentre;
    const to = inward ? towardCentre : id;

    return drawSpark(layer, points[from], points[to], at(from), Math.abs(at(to) - at(from)), strength(points[from], points[to]));
  });
  const flashes = links.flatMap((next, id) => (next.size === OUTER_CORNER_LINKS ? [flashCorner(layer, points[id], at(id), strength(points[id], points[id]))] : []));

  Promise.allSettled([...sparks, ...flashes]).then(() => layer.remove());

  return { points, links, at };
}

function holdLens(duration: number) {
  holdingLens = true;
  jumpTo(slots.indexOf(login));
  setTimeout(() => {
    holdingLens = false;

    if (inspecting && !dialog.open && mouse) {
      moveLens(mouse.x, mouse.y);
    }
  }, duration);
}

function revealModules(unlocked: HTMLElement[]) {
  holdLens(SIGNAL_MS + FADE_MS);

  if (reducedMotion.matches) {
    return;
  }

  sendSignals(false);
  unlocked.forEach((element) => element.animate([{ opacity: 0, transform: 'scale(0.92)' }, {}], { duration: FADE_MS, delay: SIGNAL_MS, easing: 'ease-out', fill: 'backwards' }));
}

function lockModules() {
  if (reducedMotion.matches) {
    applyAccess();
    return;
  }

  const locked = SIGNAL_MS + FADE_MS;
  holdLens(locked + LOCK_FADE_MS);
  sendSignals(true);
  const fades = slots
    .filter((module): module is Module => module !== null && module !== login && !module.element.hidden)
    .map((module) => module.element.animate([{}, { opacity: 0, transform: 'scale(0.92)' }], { duration: FADE_MS, delay: SIGNAL_MS, easing: 'ease-in', fill: 'forwards' }));
  login.element.animate([{ opacity: 0 }, { opacity: 0, offset: locked / (locked + LOCK_FADE_MS) }, { opacity: 1 }], { duration: locked + LOCK_FADE_MS });
  setTimeout(() => {
    applyAccess();
    fades.forEach((fade) => fade.cancel());
  }, locked);
}

const desktop = matchMedia('(min-width: 1080px)');
let lensRadius = 0;
const centres: { x: number; y: number }[] = [];

function layout() {
  const rows = slots.length / COLUMNS;
  const gap = desktop.matches ? 7 : 6;
  const margins = getComputedStyle(world);
  const widthLeft = desktop.matches ? innerWidth - header.offsetWidth - parseFloat(margins.marginRight) : innerWidth - PHONE_GUTTER;
  const heightLeft = desktop.matches ? innerHeight - world.offsetTop - parseFloat(margins.marginBottom) : Infinity;
  const widestHex = desktop.matches ? 392 : 156;
  const widthFit = (widthLeft - (COLUMNS - 1) * gap) / COLUMNS;
  const heightFit = (heightLeft - (rows - 1) * gap) / (0.75 * (rows - 1) + 1) / HEX_HEIGHT_PER_WIDTH;
  const hexWidth = Math.min(widestHex, widthFit, heightFit);
  const hexHeight = hexWidth * HEX_HEIGHT_PER_WIDTH;
  lensRadius = LENS_RADIUS_PER_HEX_HEIGHT * hexHeight;
  comb.dataset.mode = desktop.matches ? 'desktop' : 'phone';
  comb.style.setProperty('--hw', `${hexWidth}px`);
  comb.style.setProperty('--hh', `${hexHeight}px`);
  document.documentElement.style.setProperty('--lens-r', `${lensRadius}px`);
  comb.style.width = `${(COLUMNS - 1) * (hexWidth + gap) + hexWidth}px`;
  comb.style.height = `${(rows - 1) * (0.75 * hexHeight + gap) + hexHeight}px`;
  centres.length = 0;
  hexes.forEach((hex, index) => {
    const row = Math.floor(index / COLUMNS);
    const column = index % COLUMNS;
    const x = (column + (row % 2) / 2 - 0.5) * (hexWidth + gap);
    const y = row * (0.75 * hexHeight + gap);
    hex.style.left = `${x}px`;
    hex.style.top = `${y}px`;
    centres[index] = { x: x + hexWidth / 2, y: y + hexHeight / 2 };
  });

  if (inspecting) {
    moveLens(lens.x, lens.y);
  }

  if (lit) {
    paintFullLights();
  }
}
addEventListener('resize', layout);

let inspecting = false;
const lens = { x: 0, y: 0 };
let activeIndex = -1;

function moveLens(x: number, y: number) {
  lens.x = Math.max(0, Math.min(innerWidth, x));
  lens.y = Math.max(0, Math.min(innerHeight, y));
  const box = comb.getBoundingClientRect();
  const lensX = lens.x - box.left;
  const lensY = lens.y - box.top;
  comb.style.setProperty('--lens-x', `${lensX}px`);
  comb.style.setProperty('--lens-y', `${lensY}px`);
  ring.style.translate = `${lens.x - lensRadius}px ${lens.y - lensRadius}px`;

  if (dialog.open || !inspecting) {
    return;
  }

  const hit = slots.findIndex((_, index) => isAvailable(index) && Math.hypot(centres[index].x - lensX, centres[index].y - lensY) <= lensRadius);
  setActiveIndex(hit);
}

function setActiveIndex(index: number) {
  if (index === activeIndex) {
    return;
  }

  if (activeIndex >= 0) {
    slots[activeIndex]!.setActive(false);
    hexes[activeIndex].classList.remove('active');
  }

  activeIndex = index;

  if (index >= 0) {
    slots[index]!.setActive(true);
    hexes[index].classList.add('active');
  }
}

function centreOf(index: number) {
  const box = comb.getBoundingClientRect();

  return { x: box.left + centres[index].x, y: box.top + centres[index].y };
}

function jumpTo(index: number) {
  if (index < 0) {
    return;
  }

  hexes[index].scrollIntoView({ block: 'nearest' });
  const centre = centreOf(index);
  moveLens(centre.x, centre.y);
}

let mouse: { x: number; y: number } | null = null;

function setInspecting(on: boolean, fromMouse = false) {
  if (on === inspecting) {
    return;
  }

  if (on) {
    setLit(false);
  }

  inspecting = on;
  document.body.classList.toggle('inspecting', on);
  world.inert = !on;
  toggle.setAttribute('aria-pressed', String(on));
  toggleLabel.textContent = on ? 'stop' : 'inspect';

  if (on && fromMouse && mouse) {
    moveLens(mouse.x, mouse.y);
    return;
  }

  if (on) {
    jumpTo(slots.findIndex((_, index) => isAvailable(index)));
    return;
  }

  if (dialog.open) {
    dialog.close();
  }

  setActiveIndex(-1);
}

toggle.addEventListener('click', (event) => setInspecting(!inspecting, event.detail > 0));

let downAt = { x: 0, y: 0 };
let dragged = false;
addEventListener('pointermove', (event) => {
  if (event.pointerType === 'mouse') {
    mouse = { x: event.clientX, y: event.clientY };
  }

  if (!inspecting || dialog.open || holdingLens) {
    return;
  }

  if (event.pointerType === 'mouse') {
    moveLens(event.clientX, event.clientY);
    return;
  }

  if (!event.buttons) {
    return;
  }

  if (Math.hypot(event.clientX - downAt.x, event.clientY - downAt.y) > TAP_SLOP) {
    dragged = true;
  }

  moveLens(event.clientX, event.clientY - TOUCH_LIFT);
});
addEventListener('pointerdown', (event) => {
  downAt = { x: event.clientX, y: event.clientY };
  dragged = false;

  if (inspecting && !dialog.open && event.pointerType !== 'mouse' && !toggle.contains(event.target as Node)) {
    moveLens(event.clientX, event.clientY - TOUCH_LIFT);
  }
});
addEventListener('scroll', () => {
  if (inspecting) {
    moveLens(lens.x, lens.y);
  }
}, { passive: true });

addEventListener('keydown', (event) => {
  if (!inspecting || dialog.open || event.altKey || event.ctrlKey || event.metaKey) {
    return;
  }

  const direction = ARROW_DIRECTIONS[event.key];

  if (direction) {
    event.preventDefault();
    moveLens(lens.x + direction.x * KEY_STEP, lens.y + direction.y * KEY_STEP);
    return;
  }

  if (event.key === 'Enter' && activeIndex >= 0) {
    event.preventDefault();
    openHex(slots[activeIndex]!, hexes[activeIndex]);
    return;
  }

  if (event.key === 'Escape') {
    setInspecting(false);
    toggle.focus();
  }
});

comb.addEventListener('focusin', (event) => {
  const index = hexes.indexOf(event.target as HTMLDivElement);

  if (index >= 0) {
    const centre = centreOf(index);
    moveLens(centre.x, centre.y);
  }
});

let openFrom: { module: Module; hex: HTMLElement } | null = null;

function openHex(module: Module, hex: HTMLElement) {
  if (!inspecting && !lit) {
    return;
  }

  openFrom = { module, hex };
  setActiveIndex(-1);
  dialog.setAttribute('aria-label', module.element.getAttribute('aria-label') ?? 'Module');
  bigHex.append(module.element);
  module.element.dataset.open = '';
  dialog.showModal();
  module.setActive(true);
  fitObserver.observe(module.element);
  fitObserver.observe(bigHex);
}

const fitObserver = new ResizeObserver(() => {
  const element = openFrom?.module.element;

  if (!element) {
    return;
  }

  const hexWidth = bigHex.offsetWidth;
  const hexHeight = bigHex.offsetHeight;
  const scale = Math.min(1, (FIT_MARGIN * hexHeight) / (element.offsetHeight + (hexHeight * element.offsetWidth) / (2 * hexWidth)));
  element.style.transform = scale < 1 ? `scale(${scale})` : '';
});

dialog.addEventListener('close', () => {
  if (!openFrom) {
    return;
  }

  const { module, hex } = openFrom;
  openFrom = null;
  fitObserver.disconnect();
  module.element.style.transform = '';
  module.setActive(lit);
  delete module.element.dataset.open;
  hex.querySelector('.hex-inner')!.append(module.element);
  hex.focus({ preventScroll: true });

  if (inspecting) {
    moveLens(lens.x, lens.y);
  }
});
dialog.querySelector('.close-hex')!.addEventListener('click', () => dialog.close());

const bulb = document.querySelector<HTMLButtonElement>('.bulb-btn')!;
const bulbLock = new AbortController();
let clicks = 0;
let visibleMs = 0;

function unlockBulb() {
  bulb.hidden = false;
  clearInterval(visibleTimer);
  bulbLock.abort();
}

const visibleTimer = setInterval(() => {
  if (document.visibilityState !== 'visible') {
    return;
  }

  visibleMs += BULB_TICK_MS;

  if (visibleMs >= BULB_VISIBLE_MS) {
    unlockBulb();
  }
}, BULB_TICK_MS);

addEventListener('click', () => {
  clicks += 1;

  if (clicks >= BULB_CLICKS) {
    unlockBulb();
  }
}, { capture: true, signal: bulbLock.signal });

addEventListener('keydown', (event) => {
  if (event.ctrlKey && event.altKey && event.shiftKey && event.code === 'KeyO') {
    unlockBulb();
  }
}, { signal: bulbLock.signal });

let lit = false;
let lightsFrame = 0;
const lightsCanvas = document.createElement('canvas');
lightsCanvas.className = 'lights';
comb.append(lightsCanvas);
const lights = lightsCanvas.getContext('2d')!;
const edgeStrokes = document.createElement('canvas');
const strokes = edgeStrokes.getContext('2d')!;

interface Edge {
  from: number;
  to: number;
}

function edgesOf(links: Set<number>[]): Edge[] {
  return links.flatMap((next, id) => [...next].filter((other) => other > id).map((other) => ({ from: id, to: other })));
}

function strokeEdges(points: Point[], edges: Edge[], brightness: (index: number) => number, width: number, reach: number) {
  edgeStrokes.width = lightsCanvas.width;
  edgeStrokes.height = lightsCanvas.height;
  strokes.setTransform(devicePixelRatio, 0, 0, devicePixelRatio, reach * devicePixelRatio, reach * devicePixelRatio);
  strokes.lineWidth = width;
  strokes.lineCap = 'round';
  edges.forEach((edge, index) => {
    strokes.globalAlpha = brightness(index);
    strokes.beginPath();
    strokes.moveTo(points[edge.from].x, points[edge.from].y);
    strokes.lineTo(points[edge.to].x, points[edge.to].y);
    strokes.stroke();
  });
}

function shine(blur: number, color: string) {
  const offset = edgeStrokes.width;
  lights.save();
  lights.setTransform(1, 0, 0, 1, 0, 0);
  lights.shadowOffsetX = offset;
  lights.shadowBlur = blur * devicePixelRatio;
  lights.shadowColor = color;
  lights.drawImage(edgeStrokes, -offset, 0);
  lights.restore();
}

function paintLights(points: Point[], edges: Edge[], brightness: (index: number) => number) {
  const width = comb.offsetWidth;
  const height = comb.offsetHeight;
  const reach = Math.max(...EDGE_GLOWS.map((glow) => glow.width / 2 + 1.5 * glow.blur));
  const lightLevel = edges.reduce((sum, _, index) => sum + brightness(index), 0) / edges.length;
  comb.style.setProperty('--light-reach', `${reach}px`);
  lightsCanvas.width = (width + 2 * reach) * devicePixelRatio;
  lightsCanvas.height = (height + 2 * reach) * devicePixelRatio;
  lights.setTransform(devicePixelRatio, 0, 0, devicePixelRatio, reach * devicePixelRatio, reach * devicePixelRatio);
  lights.fillStyle = `rgba(0, 0, 0, ${DARK_VEIL - (DARK_VEIL - LIT_VEIL) * lightLevel})`;
  lights.fillRect(0, 0, width, height);
  EDGE_GLOWS.forEach((glow) => {
    strokeEdges(points, edges, brightness, glow.width, reach);
    shine(glow.blur, `rgba(${LIGHT_COLOR}, ${glow.alpha})`);
  });
}

function paintFullLights() {
  const { points, links } = honeycomb();
  paintLights(points, edgesOf(links), () => 1);
}

function switchLights(on: boolean) {
  cancelAnimationFrame(lightsFrame);

  if (reducedMotion.matches) {
    paintFullLights();
    comb.classList.toggle('lit', on);
    return;
  }

  const { points, links, at } = sendSignals(!on);
  const edges = edgesOf(links);
  const startsAt = edges.map((edge) => Math.min(at(edge.from), at(edge.to)));
  const start = performance.now();
  const frame = () => {
    const elapsed = performance.now() - start;
    paintLights(points, edges, (index) => {
      const progress = Math.min(1, Math.max(0, (elapsed - startsAt[index]) / LIGHT_FADE_MS));

      return on ? progress : 1 - progress;
    });

    if (elapsed < SIGNAL_MS + LIGHT_FADE_MS) {
      lightsFrame = requestAnimationFrame(frame);
      return;
    }

    comb.classList.toggle('lit', on);
  };
  frame();
  comb.classList.add('lit');
}

function setLit(on: boolean) {
  if (on === lit) {
    return;
  }

  lit = on;
  bulb.setAttribute('aria-pressed', String(on));
  world.inert = !on && !inspecting;
  setModulesActive(on);
  switchLights(on);
}

function setModulesActive(active: boolean) {
  slots.forEach((module, index) => {
    if (module) {
      module.setActive(active && isAvailable(index));
    }
  });
}

bulb.addEventListener('click', () => {
  setInspecting(false);
  setLit(!lit);
});

function keepHinting(button: HTMLButtonElement, idle: () => boolean) {
  setTimeout(() => {
    if (idle()) {
      button.animate([{
        offset: 0.5,
        color: `rgb(${LIGHT_COLOR})`,
        borderColor: `rgba(${LIGHT_COLOR}, 0.6)`,
        boxShadow: `0 0 12px rgba(${LIGHT_COLOR}, 0.35)`,
      }], { duration: HINT_GLOW_MS, easing: 'ease-in-out' });
    }

    keepHinting(button, idle);
  }, HINT_MIN_MS + Math.random() * (HINT_MAX_MS - HINT_MIN_MS));
}

keepHinting(toggle, () => !inspecting && !lit);
keepHinting(bulb, () => !bulb.hidden && !lit);

document.querySelector<HTMLElement>('.inspect-bar')!.hidden = false;
applyAccess();
layout();
