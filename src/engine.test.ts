import { describe, expect, it } from 'vitest';
import { advance, STEP_BUDGET } from './engine';

describe('advance (playback step)', () => {
  it('stops a never-ending algorithm at the budget', () => {
    const forever = (function* () {
      for (;;) {
        yield 'step';
      }
    })();
    const state = { steps: 0 };
    let outcome = advance(forever, () => {}, state);

    for (let call = 0; outcome === 'step' && call < STEP_BUDGET + 10; call++) {
      outcome = advance(forever, () => {}, state);
    }

    expect(outcome).toBe('budget');
    expect(state.steps).toBe(STEP_BUDGET);
  });

  it('applies each operation once, in order, and reports the end', () => {
    const seen: number[] = [];
    const numbers = [1, 2, 3][Symbol.iterator]() as Generator<number>;
    const state = { steps: 0 };
    const results = [1, 2, 3, 4].map(() => advance(numbers, (value) => seen.push(value), state));
    expect(results).toEqual(['step', 'step', 'step', 'done']);
    expect(seen).toEqual([1, 2, 3]);
    expect(state.steps).toBe(3);
  });

  it('lets an algorithm error reach the caller', () => {
    const broken = (function* () {
      throw new Error('boom');
    })();
    expect(() => advance(broken, () => {}, { steps: 0 })).toThrow('boom');
  });
});
