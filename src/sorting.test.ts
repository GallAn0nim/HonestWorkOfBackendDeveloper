import { describe, expect, it } from 'vitest';
import { parseInput, replay, validate, type Slot, type SortStep } from './sorting';

describe('validate', () => {
  it('rejects unordered arrays and accepts ordered ones that lost values', () => {
    expect(validate([1, 2, 3], [2, 1, 3]).valid).toBe(false);
    expect(validate([1, 2, 3], [1, 2])).toEqual({ ordered: true, preserved: false, valid: true });
  });
});

describe('replay (steps from the .NET API)', () => {
  it('applies writes and removals to the array', () => {
    const items: Slot[] = [2, 1, 3];
    const steps: SortStep[] = [
      { type: 'compare', index: 0, otherIndex: 1 },
      { type: 'write', index: 0, value: 1 },
      { type: 'write', index: 1, value: 2 },
      { type: 'remove', index: 2 },
    ];
    expect([...replay(steps, items)]).toEqual(steps);
    expect(items).toEqual([1, 2, null]);
  });

  it.each([
    { name: 'not a list', steps: { steps: [] } },
    { name: 'index out of range', steps: [{ type: 'write', index: 12, value: 5 }] },
    { name: 'unknown step', steps: [{ type: 'drop', index: 0 }] },
    { name: 'write without value', steps: [{ type: 'write', index: 0 }] },
  ])('rejects $name', ({ steps }) => {
    expect(() => [...replay(steps, Array<Slot>(12).fill(50))]).toThrow();
  });
});

describe('parseInput (input from the API)', () => {
  it('accepts the expected count of integers in 0-100', () => {
    expect(parseInput([10, 89, 50], 3)).toEqual([10, 89, 50]);
  });

  it.each([{ input: [1, 2] }, { input: [1, 2.5, 3] }, { input: [1, -1, 3] }, { input: [1, 101, 3] }, { input: '1,2,3' }])('rejects $input', ({ input }) => {
    expect(() => parseInput(input, 3)).toThrow();
  });
});
