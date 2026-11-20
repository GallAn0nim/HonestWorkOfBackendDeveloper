import { api, ApiError, helpButtons, helpPopovers } from './engine';

const ROTATE_MS = 8000;
const API_TIMEOUT_MS = 3000;
const FADE_MS = 300;

interface Quote {
  id: number;
  text: string;
}

const markup = `
  <div class="mod-head">
    <span class="cell-only">08 / (IN)SANITY</span><span class="open-only">08 / (IN)SANITY CHECK</span>
    ${helpButtons('quote', '(In)Sanity Check')}
  </div>
  <blockquote class="quote"></blockquote>
  <div class="mod-actions open-only"><button type="button" class="next-quote">Show another</button></div>
  <div class="mod-status"><span class="status" role="status"></span></div>
  ${helpPopovers('quote', '<p>A short break between the serious hexes. A random quote about the life of a developer, a new one every few seconds. Click the quote or Show another to skip ahead.</p>', `
    <p>GET /api/quotes/random picks one row from PostgreSQL in random order and skips the quote you are reading, so you never get the same one twice in a row.</p>
    <p class="info-foot">Real PostgreSQL rows · no hardcoded list on the page</p>`)}`;

export const createQuoteModule = () => {
  const reducedMotion = matchMedia('(prefers-reduced-motion: reduce)');
  const element = document.createElement('section');
  element.className = 'mod mod-quote';
  element.setAttribute('aria-label', '(In)Sanity Check');
  element.innerHTML = markup;

  const quote = element.querySelector<HTMLElement>('.quote')!;
  const status = element.querySelector('.status')!;

  let active = false;
  let hovered = false;
  let timer = 0;
  let request: AbortController | null = null;
  let current: Quote | null = null;

  const isOpen = () => 'open' in element.dataset;

  function setStatus(text: string, failed = false) {
    status.textContent = text;

    if (failed) {
      element.dataset.result = 'failed';
      return;
    }

    delete element.dataset.result;
  }

  function schedule() {
    clearTimeout(timer);
    timer = 0;

    if (active && !hovered && !document.hidden) {
      timer = setTimeout(next, ROTATE_MS);
    }
  }

  function show(next: Quote) {
    current = next;
    quote.textContent = next.text;
    setStatus(hovered ? 'PAUSED' : '');

    if (!reducedMotion.matches) {
      quote.animate([{ opacity: 0 }, { opacity: 1 }], { duration: FADE_MS, easing: 'ease-out' });
    }
  }

  async function next() {
    clearTimeout(timer);
    request?.abort();
    const controller = new AbortController();
    request = controller;
    const query = current ? `?except=${current.id}` : '';

    try {
      show(await api(`/api/quotes/random${query}`, AbortSignal.any([controller.signal, AbortSignal.timeout(API_TIMEOUT_MS)])));
    } catch (error) {
      if (controller.signal.aborted) {
        return;
      }

      setStatus(error instanceof ApiError && error.detail ? error.detail : '✕ API UNREACHABLE', true);
    }

    schedule();
  }

  function pause(paused: boolean) {
    hovered = paused;

    if (!element.dataset.result) {
      setStatus(paused ? 'PAUSED' : '');
    }

    schedule();
  }

  quote.addEventListener('click', () => {
    if (isOpen()) {
      next();
    }
  });
  element.querySelector('.next-quote')!.addEventListener('click', next);
  quote.addEventListener('pointerenter', () => pause(true));
  quote.addEventListener('pointerleave', () => pause(false));
  document.addEventListener('visibilitychange', schedule);

  return {
    element,
    setActive(on: boolean) {
      if (on === active) {
        return;
      }

      active = on;

      if (!on) {
        request?.abort();
        schedule();
        return;
      }

      if (current) {
        schedule();
        return;
      }

      next();
    },
  };
};
