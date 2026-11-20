import { describe, expect, it } from 'vitest';
import { checkRace, fromApi, replay, replayGeneration, startGeneration, toRacer, validate, wallPath, type GenerationStep } from './maze';

const corner = { width: 2, height: 2, cells: [5, 3, 5, 6], entrance: 0, exit: 2, seed: 1 };
const route = [0, 1, 3, 2];

describe('validate', () => {
  it('accepts the route through the maze', () => {
    expect(validate(corner, route)).toEqual({ reaches: true, connected: true, valid: true });
  });

  it('rejects paths that skip, stop early, leave the grid or cross walls', () => {
    expect(validate(corner, []).valid).toBe(false);
    expect(validate(corner, route.slice(0, -1)).valid).toBe(false);
    expect(validate(corner, [0, 3, 2]).valid).toBe(false);
    expect(validate(corner, [0, -1, 2]).valid).toBe(false);
    expect(validate(corner, [0, 2]).valid).toBe(false);
  });
});

describe('replay', () => {
  it.each([
    { name: 'not a list', steps: { steps: [] } },
    { name: 'index out of range', steps: [{ type: 'visit', index: 900 }] },
    { name: 'unknown step', steps: [{ type: 'jump', index: 0 }] },
  ])('rejects $name', ({ steps }) => {
    expect(() => [...replay(steps, 900)]).toThrow();
  });
});

describe('fromApi (maze from the API)', () => {
  it('keeps a well-formed maze', () => {
    expect(fromApi(corner)).toEqual(corner);
  });

  it.each([
    { name: 'not a maze', body: { grid: [] } },
    { name: 'missing cells', body: { ...corner, cells: [5, 3, 5] } },
    { name: 'unknown wall', body: { ...corner, cells: [5, 3, 5, 16] } },
    { name: 'exit outside the grid', body: { ...corner, exit: 4 } },
    { name: 'exit on the entrance', body: { ...corner, exit: 0 } },
    { name: 'missing seed', body: { ...corner, seed: undefined } },
  ])('rejects $name', ({ body }) => {
    expect(() => fromApi(body)).toThrow();
  });
});

describe('wallPath', () => {
  it('draws every wall once and leaves the entrance and exit open', () => {
    expect(wallPath(corner)).toBe('M0 0h1M1 0h1M2 0v1M0 1h1M0 2h1M1 2h1M2 1v1');
  });
});

describe('generation', () => {
  const steps: GenerationStep[] = [
    { type: 'carve', cell: 0, side: 2 },
    { type: 'carve', cell: 1, side: 4 },
    { type: 'carve', cell: 3, side: 8 },
    { type: 'door', cell: 0, side: 8 },
    { type: 'door', cell: 2, side: 8 },
  ];

  const build = (played: GenerationStep[]) => {
    const generation = startGeneration(corner);
    played.forEach(generation.apply);

    return generation;
  };

  it('rebuilds the served maze from a walled grid', () => {
    const generation = build([...replayGeneration(steps, corner)]);
    expect(generation.walls).toEqual(corner.cells);
    expect(generation.check()).toEqual({ connected: true, matches: true, valid: true });
  });

  it('fails when a cell is never carved into or the walls differ', () => {
    expect(build(steps.slice(1)).check()).toEqual({ connected: false, matches: false, valid: false });
    expect(build([...steps, { type: 'open', cell: 0, side: 4 }]).check()).toEqual({ connected: true, matches: false, valid: false });
  });

  it.each([
    { name: 'not a list', steps: { steps: [] } },
    { name: 'cell out of range', steps: [{ type: 'carve', cell: 4, side: 2 }] },
    { name: 'unknown side', steps: [{ type: 'carve', cell: 0, side: 3 }] },
    { name: 'unknown step', steps: [{ type: 'dig', cell: 0, side: 2 }] },
    { name: 'carving out of the grid', steps: [{ type: 'carve', cell: 0, side: 1 }] },
    { name: 'a door between two cells', steps: [{ type: 'door', cell: 0, side: 2 }] },
  ])('rejects $name', ({ steps }) => {
    expect(() => [...replayGeneration(steps, corner)]).toThrow();
  });
});

describe('checkRace', () => {
  const fast = { key: 'fast', visits: [0, 1, 3, 2], path: route };
  const slow = { key: 'slow', visits: [0, 1, 0, 1, 3, 2], path: route };
  const lost = { key: 'lost', visits: [0, 1], path: [0, 1] };

  it('agrees when the API names the finisher with the fewest visits', () => {
    expect(checkRace(corner, [slow, fast, lost], ['fast'])).toEqual({ valid: true, finishLine: 4 });
  });

  it('counts a tie as two winners', () => {
    expect(checkRace(corner, [fast, { ...fast, key: 'twin' }], ['fast', 'twin']).valid).toBe(true);
  });

  it('rejects a winner that was slower or never reached the exit', () => {
    expect(checkRace(corner, [slow, fast], ['slow']).valid).toBe(false);
    expect(checkRace(corner, [fast, lost], ['lost']).valid).toBe(false);
  });

  it('runs to the longest walk when nobody reaches the exit', () => {
    expect(checkRace(corner, [lost, { ...lost, key: 'other', visits: [0, 1, 0] }], [])).toEqual({ valid: true, finishLine: 3 });
  });
});

describe('toRacer', () => {
  it('splits the solver steps into visits and the reported path', () => {
    expect(toRacer('bfs', [{ type: 'visit', index: 0 }, { type: 'visit', index: 1 }, { type: 'path', index: 0 }])).toEqual({ key: 'bfs', visits: [0, 1], path: [0] });
  });
});
