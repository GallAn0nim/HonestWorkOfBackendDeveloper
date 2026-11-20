export type Slot = number | null;
export type SortStep =
  | { type: 'compare'; index: number; otherIndex: number }
  | { type: 'write'; index: number; value: number }
  | { type: 'remove'; index: number };

export type Algorithm = { key: string; name: string; big: string; info: string };

export const ALGORITHMS: Algorithm[] = [
  { key: 'bubble', name: 'Bubble Sort', big: 'O(n²)', info: 'The classic. Simple and visually satisfying.' },
  { key: 'selection', name: 'Selection Sort', big: 'O(n²)', info: 'Finds the smallest value and puts it in place, again and again.' },
  { key: 'insertion', name: 'Insertion Sort', big: 'O(n²)', info: 'Builds the sorted part one value at a time.' },
  { key: 'quick', name: 'Quick Sort', big: 'O(n log n)', info: 'Partitions around a pivot, then sorts each side.' },
  { key: 'merge', name: 'Merge Sort', big: 'O(n log n)', info: 'Divide. Conquer. Repeat.' },
  { key: 'heap', name: 'Heap Sort', big: 'O(n log n)', info: 'Builds a heap, then keeps pulling the largest value off the top.' },
  { key: 'shell', name: 'Shell Sort', big: 'gap-dependent', info: 'Values jump across the array using shrinking gaps.' },
  { key: 'cycle', name: 'Cycle Sort', big: 'O(n²)', info: 'Writes each value at most once.' },
  { key: 'bogo', name: 'Bogo Sort', big: 'O((n+1)!)', info: 'Absolute chaos. Shuffles until sorted.' },
  { key: 'bozo', name: 'Bozo Sort', big: 'O((n+1)!)', info: 'Swaps two random values until sorted.' },
  { key: 'purge', name: 'Purge Sort', big: 'O(n)', info: 'Extremely fast at the minor expense of deleting data.' },
];

export function validate(input: readonly number[], output: readonly Slot[]) {
  const kept = output.filter((value): value is number => value !== null);
  const ordered = kept.every((value, index) => index === 0 || kept[index - 1] <= value);
  const sorted = (values: readonly number[]) => [...values].sort((left, right) => left - right).join();
  const preserved = kept.length === input.length && sorted(kept) === sorted(input);

  return { ordered, preserved, valid: ordered };
}

export function parseInput(input: unknown, count: number): number[] {
  if (Array.isArray(input) && input.length === count && input.every((value) => Number.isInteger(value) && value >= 0 && value <= 100)) {
    return input;
  }

  throw new Error(`Bad input from API: ${JSON.stringify(input)}`);
}

export function* replay(steps: unknown, items: Slot[]): Generator<SortStep> {
  if (!Array.isArray(steps)) {
    throw new Error('API steps are not a list');
  }

  const isSlot = (index: unknown): index is number => Number.isInteger(index) && (index as number) >= 0 && (index as number) < items.length;

  for (const step of steps as SortStep[]) {
    if (step?.type === 'compare' && isSlot(step.index) && isSlot(step.otherIndex)) {
      yield step;
    } else if (step?.type === 'write' && isSlot(step.index) && Number.isInteger(step.value)) {
      items[step.index] = step.value;
      yield step;
    } else if (step?.type === 'remove' && isSlot(step.index)) {
      items[step.index] = null;
      yield step;
    } else {
      throw new Error(`Bad step from API: ${JSON.stringify(step)}`);
    }
  }
}
