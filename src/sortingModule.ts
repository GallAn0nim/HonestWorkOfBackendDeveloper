import { api, createModule, yesNo, type Run } from './engine';
import { ALGORITHMS, parseInput, replay, validate, type SortStep, type Slot } from './sorting';

const ITEM_COUNT = 12;

export const createSortingEngine = () =>
  createModule<SortStep>({
    id: 'sort',
    label: 'Sorting Engine',
    cell: '03 / SORT',
    open: '03 / SORTING ENGINE',
    about: `A visualization of different sorting algorithms putting the same ${ITEM_COUNT} random values in ascending order, one compare and one write at a time. Some are fast, some are slow, a few are jokes. Each run picks another algorithm.`,
    tech: `The .NET API sorts ${ITEM_COUNT} random values with the algorithm below and records every compare and write. The page only replays those steps, then checks that the result is ordered and that no value was lost.`,
    success: '✓ VALID SORT',
    view: `<div class="bars">${'<span></span>'.repeat(ITEM_COUNT)}</div><div class="marks open-only" aria-hidden="true">${'<span></span>'.repeat(ITEM_COUNT)}</div>`,
    counters: [
      { key: 'c', label: 'COMPARES' },
      { key: 'w', label: 'WRITES' },
      { key: 'n', label: 'ITEMS' },
    ],
    tickMs: 100,
    newRun(element): Run<SortStep> {
      const algorithm = ALGORITHMS[Math.floor(Math.random() * ALGORITHMS.length)];
      const bars = [...element.querySelector('.bars')!.children] as HTMLElement[];
      const marks = [...element.querySelector('.marks')!.children] as HTMLElement[];
      let input: number[] = [];
      let items: Slot[] = [];
      let current = -1;
      let compared = -1;
      let compares = 0;
      let writes = 0;

      return {
        name: algorithm.name,
        big: algorithm.big,
        info: algorithm.info,
        async remote(signal) {
          input = parseInput((await api(`/api/sort/random?count=${ITEM_COUNT}`, signal)).input, ITEM_COUNT);
          items = [...input];
          const { steps } = await api('/api/sort', signal, { algorithm: algorithm.key, input });

          return replay(steps, items);
        },
        onStep(step) {
          current = step.index;
          compared = step.type === 'compare' ? step.otherIndex : -1;

          if (step.type === 'compare') {
            compares++;
          }

          if (step.type === 'write') {
            writes++;
          }
        },
        check() {
          const verdict = validate(input, items);

          return { valid: verdict.valid, detail: `ORDERED: ${yesNo(verdict.ordered)} · VALUES PRESERVED: ${yesNo(verdict.preserved)}` };
        },
        counters: () => [String(compares), String(writes), input.length ? `${items.filter((value) => value !== null).length} / ${ITEM_COUNT}` : '—'],
        draw() {
          bars.forEach((bar, index) => {
            const value = items[index];
            bar.style.height = value == null ? '' : `${value}%`;
            bar.className = barClass(value, index === current, index === compared);
            marks[index].textContent = value === null ? '✕' : '';
          });
        },
      };
    },
  });

function barClass(value: Slot, isCurrent: boolean, isCompared: boolean) {
  if (value === null) {
    return 'removed';
  }

  if (isCurrent) {
    return 'cur';
  }

  return isCompared ? 'cmp' : '';
}
