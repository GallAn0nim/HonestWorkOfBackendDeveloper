import { expect, test, type Page } from '@playwright/test';

const NAME = 'Oskar Gruszczyński';
const sleep = (milliseconds: number) => new Promise((resolve) => setTimeout(resolve, milliseconds));
const status = (page: Page) => page.locator('.mod-sort .status').textContent();
const compares = async (page: Page) => Number(await page.locator('.mod-sort [data-k=c]').textContent());
const inspect = (page: Page) => page.locator('.inspect-btn').click();
const sortHex = (page: Page) => page.locator('.hex[aria-label="Open Sorting Engine"]');
const mazeHex = (page: Page) => page.locator('.hex[aria-label="Open Maze"]');

async function hexCentre(page: Page) {
  const box = (await sortHex(page).boundingBox())!;

  return { x: box.x + box.width / 2, y: box.y + box.height / 2 };
}

const FINISHED = /VALID SORT|CONTRACT FAILED|STOPPED/;

test.beforeEach(async ({ page }) => {
  await page.request.post('/api/auth/login', { data: { username: 'Test', password: 'test' } });
});

async function finishRun(page: Page, algorithm: string) {
  for (let i = 0; i < 150 && (await page.locator('.mod-sort .alg').textContent()) !== algorithm; i++) {
    await page.getByRole('button', { name: 'Run another' }).click();
  }

  await expect(page.locator('.mod-sort .alg')).toHaveText(algorithm);
  await page.getByRole('button', { name: 'Run to end' }).click();
  await expect(page.locator('.mod-sort .status')).toHaveText(FINISHED);
}

test('1. Without JS: name, tagline and three working links', async ({ browser }) => {
  const context = await browser.newContext({ javaScriptEnabled: false });
  const page = await context.newPage();
  await page.goto('/');
  await expect(page.getByRole('heading', { level: 1 })).toHaveText(NAME);
  await expect(page.getByText('Honest work of a backend developer')).toBeVisible();
  await expect(page.getByRole('link', { name: 'GitHub' })).toHaveAttribute('href', 'https://github.com/GallAn0nim');
  await expect(page.getByRole('link', { name: 'LinkedIn' })).toHaveAttribute('href', /^https:\/\/www\.linkedin\.com\/in\//);
  await expect(page.getByRole('link', { name: 'Curriculum Vitae' })).toHaveAttribute('href', '/api/cv');
  await expect(page.getByRole('link', { name: 'Contact me' })).toHaveAttribute('href', /^mailto:.+@.+/);
  await expect(page.locator('.inspect-btn')).toBeHidden();
  await context.close();
});

test('1b. CV link target exists', async ({ request }) => {
  expect((await request.get('/api/cv')).headers()['content-type']).toContain('application/pdf');
});

test('1c. CV opens for viewing in a new tab instead of downloading', async ({ page, request }) => {
  await page.goto('/');
  await expect(page.getByRole('link', { name: 'Curriculum Vitae' })).toHaveAttribute('target', '_blank');
  expect((await request.get('/api/cv')).headers()['content-disposition']).toMatch(/^inline;.*CV-en\.pdf/);
});

test('2. First Tab focuses GitHub; nothing hidden gets focus while inspection is off', async ({ page }) => {
  await page.goto('/');
  await page.keyboard.press('Tab');
  await expect(page.getByRole('link', { name: 'GitHub' })).toBeFocused();

  for (let i = 0; i < 8; i++) {
    await page.keyboard.press('Tab');
    expect(await page.evaluate(() => !!document.activeElement?.closest('.world, dialog'))).toBe(false);
  }
});

test.describe('3. Inspection on and off', () => {
  test('mouse: click on, click off, Esc off', async ({ page }) => {
    await page.goto('/');
    const button = page.locator('.inspect-btn');
    await button.click();
    await expect(button).toHaveAttribute('aria-pressed', 'true');
    await expect(page.locator('.inspect-label')).toHaveText('stop');
    await button.click();
    await expect(button).toHaveAttribute('aria-pressed', 'false');
    await expect(page.locator('.inspect-label')).toHaveText('inspect');
    await button.click();
    await page.keyboard.press('Escape');
    await expect(button).toHaveAttribute('aria-pressed', 'false');
    expect(await page.locator('.world').evaluate((element) => (element as HTMLElement).inert)).toBe(true);
  });

  test('keyboard: Enter on, arrows move, Tab reaches hex, Enter opens, Esc closes innermost first', async ({ page }) => {
    await page.goto('/');
    await page.locator('.inspect-btn').focus();
    await page.keyboard.press('Enter');
    await expect(page.locator('.inspect-btn')).toHaveAttribute('aria-pressed', 'true');
    const lensX = () => page.locator('.comb').evaluate((element) => parseFloat((element as HTMLElement).style.getPropertyValue('--lens-x')));
    const startX = await lensX();
    await page.keyboard.press('ArrowLeft');
    expect(await lensX()).toBeLessThan(startX);

    for (let i = 0; i < 5 && !(await sortHex(page).evaluate((element) => element === document.activeElement)); i++) {
      await page.keyboard.press('Tab');
    }

    await expect(sortHex(page)).toBeFocused();
    await page.keyboard.press('Enter');
    await expect(page.locator('dialog.open-hex')).toBeVisible();
    await page.getByRole('button', { name: /^About / }).click();
    await expect(page.locator('#sort-about')).toBeVisible();
    await page.keyboard.press('Escape');
    await expect(page.locator('#sort-about')).toBeHidden();
    await expect(page.getByRole('button', { name: /^About / })).toBeFocused();
    await expect(page.locator('dialog.open-hex')).toBeVisible();
    await page.keyboard.press('Escape');
    await expect(page.locator('dialog.open-hex')).toBeHidden();
    await expect(sortHex(page)).toBeFocused();
    await page.keyboard.press('Escape');
    await expect(page.locator('.inspect-btn')).toHaveAttribute('aria-pressed', 'false');
  });
});

test.describe('touch', () => {
  test.use({ viewport: { width: 390, height: 844 }, hasTouch: true, isMobile: true });

  test('3/4. tap toggles; the lens is drawn ~80 px above the finger', async ({ page }) => {
    await page.goto('/');
    await page.locator('.inspect-btn').tap();
    await expect(page.locator('.inspect-btn')).toHaveAttribute('aria-pressed', 'true');
    const finger = { x: 195, y: 600 };
    await page.evaluate(({ x, y }) => {
      const touch = (type: string, offsetY: number) => document.elementFromPoint(x, y + offsetY)!.dispatchEvent(new PointerEvent(type, { pointerType: 'touch', clientX: x, clientY: y + offsetY, buttons: 1, bubbles: true }));
      touch('pointerdown', 0);
      touch('pointermove', 20);
    }, finger);
    const ring = (await page.locator('.lens-ring').boundingBox())!;
    expect(ring.x + ring.width / 2).toBeCloseTo(finger.x, 0);
    expect(ring.y + ring.height / 2).toBeCloseTo(finger.y + 20 - 80, 0);
    await page.locator('.inspect-btn').tap();
    await expect(page.locator('.inspect-btn')).toHaveAttribute('aria-pressed', 'false');
  });

  test('links have >= 44 px tap height', async ({ page }) => {
    await page.goto('/');

    for (const link of await page.locator('nav a').all()) {
      expect((await link.boundingBox())!.height).toBeGreaterThanOrEqual(44);
    }
  });
});

test('5. No timers before the first inspection or after it ends', async ({ page }) => {
  await page.goto('/');
  await page.waitForTimeout(800);
  expect(await status(page)).toBe('IDLE');
  expect(await compares(page)).toBe(0);
  await inspect(page);
  const centre = await hexCentre(page);
  await page.mouse.move(centre.x, centre.y);
  await page.waitForTimeout(800);
  expect(await compares(page)).toBeGreaterThan(0);
  await inspect(page);
  const paused = await compares(page);
  await page.waitForTimeout(800);
  expect(await compares(page)).toBe(paused);
});

test('6. Leaving the module pauses it; returning resumes the same run', async ({ page }) => {
  await page.goto('/');
  await inspect(page);
  const centre = await hexCentre(page);
  await page.mouse.move(centre.x, centre.y);
  await page.waitForTimeout(600);
  const algorithm = await page.locator('.mod-sort .alg').textContent();
  await page.mouse.move(20, 20);
  await expect(page.locator('.mod-sort .status')).toHaveText('PAUSED');
  const paused = await compares(page);
  await page.waitForTimeout(600);
  expect(await compares(page)).toBe(paused);
  await page.mouse.move(centre.x, centre.y);
  await expect(page.locator('.mod-sort .alg')).toHaveText(algorithm!);
  await expect(page.locator('.mod-sort .status')).toHaveText('● RUNNING');
  await expect.poll(() => compares(page)).toBeGreaterThan(paused);
});

test.describe('7/11. results and reduced motion', () => {
  test.use({ contextOptions: { reducedMotion: 'reduce' } });

  test('11. no autoplay; Step and Run to end work', async ({ page }) => {
    await page.goto('/');
    await inspect(page);
    await page.waitForTimeout(600);
    expect(await status(page)).toBe('IDLE');
    await sortHex(page).click();
    await page.getByRole('button', { name: 'Step', exact: true }).click();
    await expect(page.locator('.mod-sort .status')).toHaveText('PAUSED');
    expect((await compares(page)) + Number(await page.locator('.mod-sort [data-k=w]').textContent())).toBe(1);
    await page.getByRole('button', { name: 'Run to end' }).click();
    await expect(page.locator('.mod-sort .status')).toHaveText(FINISHED);
  });

  for (const algorithm of ['Bubble Sort', 'Merge Sort']) {
    test(`7. ${algorithm} ends in ✓ VALID SORT`, async ({ page }) => {
      await page.goto('/');
      await inspect(page);
      await sortHex(page).click();
      await finishRun(page, algorithm);
      await expect(page.locator('.mod-sort .status')).toHaveText('✓ VALID SORT');
      await expect(page.locator('.mod-sort .detail')).toHaveText('ORDERED: YES · VALUES PRESERVED: YES');
      await expect(page.locator('.mod-sort [data-k=n]')).toHaveText('12 / 12');
    });
  }

  test('7. Bogo Sort stops at the step budget', async ({ page }) => {
    await page.goto('/');
    await inspect(page);
    await sortHex(page).click();
    await finishRun(page, 'Bogo Sort');
    await expect(page.locator('.mod-sort .status')).toHaveText('STOPPED: budget');
  });

  test('7. Purge Sort ends in ✓ VALID SORT with removed values marked', async ({ page }) => {
    await page.goto('/');
    await inspect(page);
    await sortHex(page).click();

    for (let i = 0; i < 5; i++) {
      await finishRun(page, 'Purge Sort');

      if ((await page.locator('.mod-sort .detail').textContent())!.endsWith('PRESERVED: NO')) {
        break;
      }

      await page.getByRole('button', { name: 'Run another' }).click();
    }

    await expect(page.locator('.mod-sort .status')).toHaveText('✓ VALID SORT');
    await expect(page.locator('.mod-sort .detail')).toHaveText('ORDERED: YES · VALUES PRESERVED: NO');
    const removed = await page.locator('.mod-sort .bars span.removed').count();
    expect(removed).toBeGreaterThan(0);
    await expect(page.locator('.mod-sort .marks span', { hasText: '✕' })).toHaveCount(removed);
    await expect(page.locator('.mod-sort .bars span')).toHaveCount(12);
    await expect(page.locator('.mod-sort [data-k=n]')).toHaveText(`${12 - removed} / 12`);
  });
});

async function textProblems(page: Page) {
  return page.evaluate(() => {
    const MIN_FONT_SIZE = 12;
    const MIN_CONTRAST = 4.5;

    const channels = (colour: string) => (colour.match(/[\d.]+/g) ?? []).map(Number);

    const linear = (channel: number) => {
      const share = channel / 255;

      return share <= 0.03928 ? share / 12.92 : ((share + 0.055) / 1.055) ** 2.4;
    };

    const luminance = (colour: number[]) => 0.2126 * linear(colour[0]) + 0.7152 * linear(colour[1]) + 0.0722 * linear(colour[2]);

    const backgroundOf = (start: Element): number[] => {
      for (let element: Element | null = start; element; element = element.parentElement) {
        const colour = channels(getComputedStyle(element).backgroundColor);

        if (colour.length === 3 || colour[3] > 0) {
          return colour;
        }
      }

      return [0, 0, 0];
    };

    const problems: string[] = [];
    const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT);

    for (let node = walker.nextNode(); node; node = walker.nextNode()) {
      const element = node.parentElement!;

      if (!node.textContent!.trim() || !element.checkVisibility({ visibilityProperty: true })) {
        continue;
      }

      if (element.closest('.world') && !element.closest('.hex.active')) {
        continue;
      }

      const style = getComputedStyle(element);
      const size = parseFloat(style.fontSize);
      const text = luminance(channels(style.color));
      const background = luminance(backgroundOf(element));
      const ratio = (Math.max(text, background) + 0.05) / (Math.min(text, background) + 0.05);

      if (size < MIN_FONT_SIZE || ratio < MIN_CONTRAST) {
        problems.push(`"${node.textContent!.trim().slice(0, 30)}" ${size}px ${ratio.toFixed(2)}:1`);
      }
    }

    return problems;
  });
}

test('9. All readable text is >= 12 px and >= 4.5:1 (off, inspecting, open, popover)', async ({ page }) => {
  await page.goto('/');
  expect(await textProblems(page)).toEqual([]);
  await inspect(page);
  await page.waitForTimeout(400);
  expect(await textProblems(page)).toEqual([]);
  await sortHex(page).click();
  expect(await textProblems(page)).toEqual([]);
  await page.getByRole('button', { name: /^About / }).click();
  expect(await textProblems(page)).toEqual([]);
});

test.describe('10. 375 px', () => {
  test.use({ viewport: { width: 375, height: 667 } });
  test('no horizontal scroll; magnifier and links reachable; every hex fully on screen', async ({ page }) => {
    await page.goto('/');
    expect(await page.evaluate(() => document.documentElement.scrollWidth)).toBeLessThanOrEqual(375);
    await expect(page.locator('.inspect-btn')).toBeInViewport({ ratio: 1 });

    for (const link of await page.locator('nav a').all()) {
      await expect(link).toBeInViewport({ ratio: 1 });
    }

    for (const hex of await page.locator('.comb .hex:not([hidden])').all()) {
      const box = (await hex.boundingBox())!;
      expect(box.x).toBeGreaterThanOrEqual(0);
      expect(box.x + box.width).toBeLessThanOrEqual(375);
    }
  });
});

test('12. Link preview tags carry the name', async ({ page }) => {
  await page.goto('/');
  await expect(page).toHaveTitle(new RegExp(`${NAME}.*backend developer`));
  await expect(page.locator('meta[property="og:title"]')).toHaveAttribute('content', new RegExp(NAME));
  await expect(page.locator('meta[name="description"]')).toHaveAttribute('content', /.+/);
});

test('13. Every hex has the same size', async ({ page }) => {
  await page.goto('/');
  const sizes = await page.locator('.comb .hex:not([hidden])').evaluateAll((elements) => elements.map((element) => `${(element as HTMLElement).offsetWidth}x${(element as HTMLElement).offsetHeight}`));
  expect(sizes).toHaveLength(7);
  expect(new Set(sizes).size).toBe(1);
});

test('Without a session only the Login hex can be opened; signing in unlocks the rest', async ({ page }) => {
  await page.request.post('/api/auth/logout');
  await page.goto('/');
  await inspect(page);
  await expect(page.locator('.comb .hex.module')).toHaveCount(1);
  await expect(page.locator('.comb .hex.module')).toHaveAttribute('aria-label', 'Open Login');
  expect((await page.request.get('/api/sort/algorithms')).status()).toBe(401);
  await page.locator('.hex[aria-label="Open Login"]').click();
  const form = page.locator('dialog .login-form');
  await form.getByLabel('Username').fill('Test');
  await form.locator('input[name=password]').fill('test');
  await form.getByRole('button', { name: 'Sign in' }).click();
  await expect(page.locator('dialog.open-hex')).toBeHidden();
  await expect(page.locator('.comb .hex.module')).toHaveCount(7);
  await expect(page.locator('.hex[aria-label="Open Login"] .signed-in')).toHaveText(/SIGNED IN\s*Test/i);
  await page.reload();
  await inspect(page);
  await expect(page.locator('.comb .hex.module')).toHaveCount(7);
});

test('Picking a suggested username fills the Remind password field and stays there', async ({ page }) => {
  await page.request.post('/api/auth/logout');
  await page.goto('/');
  await inspect(page);
  await page.locator('.hex[aria-label="Open Login"]').click();
  await page.getByRole('button', { name: 'Forgot username or password?' }).click();
  const form = page.locator('dialog .recover-form');
  await form.getByLabel('Username').fill('adam');
  await form.getByRole('button', { name: 'Remind password' }).click();
  await form.getByRole('button', { name: 'Admin', exact: true }).click();
  await expect(form).toBeVisible();
  await expect(form.getByLabel('Username')).toHaveValue('Admin');
  await expect(form.getByLabel('Username')).toBeFocused();
});

test('Repeated failed sign-ins escalate the forgot hint and then switch to recovery', async ({ page }) => {
  await page.request.post('/api/auth/logout');
  await page.goto('/');
  await inspect(page);
  await page.locator('.hex[aria-label="Open Login"]').click();
  const form = page.locator('dialog .login-form');
  const forgot = form.getByRole('button', { name: 'Forgot username or password?' });
  const about = page.locator('#login-about');
  const fontSize = () => forgot.evaluate((button) => parseFloat(getComputedStyle(button).fontSize));
  const failSignIn = async () => {
    await form.getByLabel('Username').fill('nobody');
    await form.locator('input[name=password]').fill('wrong');
    await form.getByRole('button', { name: 'Sign in' }).click();
    await expect(page.locator('dialog .mod-login .status')).not.toHaveText('SENDING…');
  };

  const normalSize = await fontSize();

  for (let attempt = 1; attempt <= 3; attempt++) {
    await failSignIn();
  }

  await expect(forgot).toHaveClass(/nudge/);
  expect(await fontSize()).toBe(normalSize);
  await failSignIn();
  const grownSize = await fontSize();
  expect(grownSize).toBeGreaterThan(normalSize);
  await failSignIn();
  expect(await fontSize()).toBeGreaterThan(grownSize);
  await expect(about).toBeHidden();
  await failSignIn();
  await expect(about).toBeVisible();
  await expect(about.locator('.forgot-hint')).not.toHaveClass(/nudge/);
  await failSignIn();
  await expect(about.locator('.forgot-hint')).toHaveClass(/nudge/);
  await failSignIn();
  await expect(page.locator('dialog .recover-form')).toBeVisible();
  await expect(page.locator('dialog .recover-note')).toHaveText(/You clearly don't remember your credentials/);
});

test('14. The lens shows one whole hex plus the edges of its neighbours', async ({ page }) => {
  await page.goto('/');
  const geometry = await page.evaluate(() => {
    const hexes = [...document.querySelectorAll<HTMLElement>('.comb .hex:not([hidden])')];
    const first = hexes[0];
    const second = hexes[1];

    return {
      lensRadius: parseFloat(getComputedStyle(document.documentElement).getPropertyValue('--lens-r')),
      width: first.offsetWidth,
      height: first.offsetHeight,
      step: second.offsetLeft - first.offsetLeft,
    };
  });
  expect(geometry.lensRadius).toBeGreaterThanOrEqual(geometry.height / 2);
  expect(geometry.lensRadius).toBeGreaterThan(geometry.step - geometry.width / 2);
  expect(geometry.lensRadius).toBeLessThan(geometry.step + geometry.width / 2);
});

test('15. Opening Sorting shows the enlarged hex; Esc or ✕ closes it and focus returns to the hex', async ({ page }) => {
  await page.goto('/');
  await inspect(page);
  await sortHex(page).click();
  const big = (await page.locator('dialog .hex.big').boundingBox())!;
  expect(Math.round(big.width)).toBe(710);
  expect(Math.round(big.height)).toBe(820);
  await expect(page.locator('dialog .mod .counters')).toBeVisible();
  await page.getByRole('button', { name: 'Close', exact: true }).click();
  await expect(page.locator('dialog.open-hex')).toBeHidden();
  await expect(sortHex(page)).toBeFocused();
  await sortHex(page).click();
  await page.keyboard.press('Escape');
  await expect(page.locator('dialog.open-hex')).toBeHidden();
  await expect(sortHex(page)).toBeFocused();
  await expect(sortHex(page).locator('.mod')).toHaveCount(1);
});

test.describe('.NET API', () => {
  test.use({ contextOptions: { reducedMotion: 'reduce' } });

  async function openInfoAfterRun(page: Page) {
    await sortHex(page).click();
    await page.getByRole('button', { name: 'Run to end' }).click();
    await expect(page.locator('.mod-sort .status')).toHaveText(FINISHED);
    await page.getByRole('button', { name: /^About / }).click();
  }

  test('a run plays operations computed by the API; nothing is sent before inspection', async ({ page }) => {
    const calls: string[] = [];
    let served: number[] = [];
    page.on('request', (request) => {
      if (request.url().includes('/api/sort')) {
        calls.push(`${request.method()} ${new URL(request.url()).pathname} ${request.postData() ?? ''}`);
      }
    });
    page.on('response', async (response) => {
      if (response.url().includes('/api/sort/random')) {
        served = (await response.json()).input;
      }
    });
    await page.goto('/');
    await page.waitForTimeout(500);
    expect(calls).toHaveLength(0);
    await inspect(page);
    await openInfoAfterRun(page);
    await expect(page.locator('.mod-sort .info-src')).toHaveText('Ran on the .NET API.');
    expect(calls).toHaveLength(2);
    expect(calls[0]).toBe('GET /api/sort/random ');
    expect(calls[1]).toMatch(/^POST \/api\/sort /);
    const posted = JSON.parse(calls[1].replace('POST /api/sort ', ''));
    expect(posted).toMatchObject({ algorithm: expect.stringMatching(/^(bubble|selection|insertion|quick|merge|heap|shell|cycle|bogo|bozo|purge)$/) });
    expect(posted.input).toEqual(served);
    expect(served).toHaveLength(12);
  });

  test('the API rejects oversized bodies with 413 before reading them', async ({ page }) => {
    const response = await page.request.post('http://localhost:5280/api/sort', { data: { algorithm: 'merge', input: Array(2000).fill(50) } });
    expect(response.status()).toBe(413);
  });

  test('the page lists exactly the algorithms the API supports', async ({ page }) => {
    await page.goto('/');
    const keys = await page.evaluate(async () => {
      const { ALGORITHMS } = await import('/src/sorting.ts');
      const { algorithms } = await (await fetch('/api/sort/algorithms')).json();

      return { page: ALGORITHMS.map((algorithm: { key: string }) => algorithm.key), api: algorithms };
    });
    expect(keys.page).toEqual(keys.api);
  });

  test('Run another while the API is slow: the old answer is thrown away', async ({ page }) => {
    let first = true;
    await page.route('**/api/sort', async (route) => {
      if (first) {
        first = false;
        await sleep(1200);
      }

      await route.continue().catch(() => {});
    });
    await page.goto('/');
    await inspect(page);
    await sortHex(page).click();
    await page.getByRole('button', { name: 'Run to end' }).click();
    await page.getByRole('button', { name: 'Run another' }).click();
    await page.getByRole('button', { name: 'Run to end' }).click();
    await expect(page.locator('.mod-sort .status')).toHaveText(FINISHED);
    const snapshot = () => Promise.all(['.status', '.detail', '[data-k=c]', '[data-k=w]', '[data-k=n]'].map((selector) => page.locator(`.mod-sort ${selector}`).textContent()));
    const before = await snapshot();
    await page.waitForTimeout(1500);
    expect(await snapshot()).toEqual(before);
  });

  test('a hanging API ends the run within one 2-second limit', async ({ page }) => {
    await page.route('**/api/sort/random*', async (route) => {
      await sleep(1500);
      await route.continue().catch(() => {});
    });
    await page.route('**/api/sort', async (route) => {
      await sleep(5000);
      await route.continue().catch(() => {});
    });
    await page.goto('/');
    await inspect(page);
    await sortHex(page).click();
    const startedAt = Date.now();
    await page.getByRole('button', { name: 'Run to end' }).click();
    await expect(page.locator('.mod-sort .status')).toHaveText('✕ API UNREACHABLE', { timeout: 6000 });
    expect(Date.now() - startedAt).toBeLessThan(2800);
    await expect(page.locator('.mod-sort .info-src')).toHaveText('The .NET API is unreachable.');
  });

  test('when the API is unreachable the run says so instead of sorting', async ({ page }) => {
    await page.route('**/api/sort**', (route) => route.abort());
    await page.goto('/');
    await inspect(page);
    await sortHex(page).click();
    await page.getByRole('button', { name: 'Run to end' }).click();
    await expect(page.locator('.mod-sort .status')).toHaveText('✕ API UNREACHABLE');
    await page.getByRole('button', { name: /^About / }).click();
    await expect(page.locator('.mod-sort .info-src')).toHaveText('The .NET API is unreachable.');
  });

  test('a malformed API answer shows ✕ ERROR instead of a wrong result', async ({ page }) => {
    await page.route('**/api/sort', (route) => route.fulfill({ json: { steps: [{ type: 'write', index: 99, value: 1 }] } }));
    await page.goto('/');
    await inspect(page);
    await sortHex(page).click();
    await page.getByRole('button', { name: 'Run to end' }).click();
    await expect(page.locator('.mod-sort .status')).toHaveText('✕ ERROR');
  });
});

test.describe('Maze', () => {
  test.use({ contextOptions: { reducedMotion: 'reduce' } });
  const maze = (page: Page) => page.locator('.mod-maze');
  const counter = async (page: Page, key: string) => Number(await maze(page).locator(`[data-k=${key}]`).textContent());
  const walkers = ['Trémaux', 'Drunk Mouse', 'Wall Hugger'];
  const mayGiveUp = ['Drunk Mouse', 'Wall Hugger'];

  test('each solver runs on the .NET API and every route it reports is valid', async ({ page }) => {
    await page.goto('/');
    await inspect(page);
    await mazeHex(page).click();
    const seen = new Set<string>();

    for (let i = 0; i < 150 && seen.size < 9; i++) {
      await page.getByRole('button', { name: 'Finish run' }).click();
      await expect(maze(page).locator('.status')).toHaveText(/^(✓ PATH FOUND|✕ FAILED)$/);
      await expect(maze(page).locator('.info-src')).toHaveText('Ran on the .NET API.');
      const algorithm = (await maze(page).locator('.alg').textContent())!;
      expect(await counter(page, 'p')).toBeGreaterThan(0);

      if ((await maze(page).locator('.status').textContent()) === '✕ FAILED') {
        expect(mayGiveUp, algorithm).toContain(algorithm);
        await expect(maze(page).locator('.detail')).toHaveText('MAZE BUILT: YES · REACHES GOAL: NO · PATH CONNECTED: YES');
      } else {
        await expect(maze(page).locator('.detail')).toHaveText('MAZE BUILT: YES · REACHES GOAL: YES · PATH CONNECTED: YES');
      }

      const visited = await counter(page, 'v');
      const explored = await counter(page, 'e');
      const backtracks = await counter(page, 'b');
      expect(explored + backtracks).toBe(visited);

      if (!walkers.includes(algorithm)) {
        expect(backtracks, algorithm).toBe(0);
      }

      seen.add(algorithm);
      await page.getByRole('button', { name: 'Run another' }).click();
    }

    expect([...seen].sort()).toEqual([
      'A* Search', 'Bidirectional BFS', 'Breadth-First Search', 'Depth-First Search', 'Dijkstra',
      'Drunk Mouse', 'Greedy Best-First', 'Trémaux', 'Wall Hugger',
    ]);
  });

  test('the maze comes from GET /api/maze/generation and is solved as served', async ({ page }) => {
    let served = { seed: -1, cells: [] as number[] };
    let posted = -2;
    page.on('response', async (response) => {
      if (response.url().includes('/api/maze/generation')) {
        served = (await response.json()).maze;
      }
    });
    page.on('request', (request) => {
      if (request.method() === 'POST' && request.url().endsWith('/api/maze')) {
        posted = JSON.parse(request.postData()!).seed;
      }
    });
    await page.goto('/');
    await inspect(page);
    await mazeHex(page).click();
    await page.getByRole('button', { name: 'Finish run' }).click();
    await expect(maze(page).locator('.info-src')).toHaveText('Ran on the .NET API.');
    expect(served.cells).toHaveLength(900);
    expect(posted).toBe(served.seed);
    await expect(maze(page).locator('.solve-maze span')).toHaveCount(900);
    expect(await maze(page).locator('.solve-maze path').getAttribute('d')).toMatch(/^M/);
  });

  test('Run another solves the same maze again; New maze builds a fresh one first', async ({ page }) => {
    const generated: string[] = [];
    const solved: number[] = [];
    page.on('request', (request) => {
      const url = new URL(request.url());

      if (url.pathname === '/api/maze/generation') {
        generated.push(url.searchParams.get('seed')!);
      }

      if (request.method() === 'POST' && url.pathname === '/api/maze') {
        solved.push(JSON.parse(request.postData()!).seed);
      }
    });
    await page.goto('/');
    await inspect(page);
    await mazeHex(page).click();

    for (const button of ['Run another', 'Run another', 'New maze']) {
      await page.getByRole('button', { name: 'Finish run' }).click();
      await expect(maze(page).locator('.detail')).toHaveText(/^MAZE BUILT: YES · /);
      await expect(maze(page).locator('[data-k=c]')).toHaveText('899');
      await expect(maze(page).locator('[data-k=l]')).toHaveText('126');
      await expect(maze(page).locator('.solve-maze :is(.entrance, .exit)')).toHaveCount(2);
      expect(await counter(page, 'p')).toBeGreaterThan(0);
      await page.getByRole('button', { name: button }).click();
    }

    await page.getByRole('button', { name: 'Finish run' }).click();
    await expect(maze(page).locator('.detail')).toHaveText(/^MAZE BUILT: YES · /);
    expect(generated).toHaveLength(2);
    expect(generated[1]).not.toBe(generated[0]);
    expect(solved.map(String)).toEqual([generated[0], generated[0], generated[0], generated[1]]);
  });

  test('Finish maze skips the build and leaves the solve to play', async ({ page }) => {
    await page.goto('/');
    await inspect(page);
    await mazeHex(page).click();
    await page.getByRole('button', { name: 'Finish maze' }).click();
    await expect(maze(page).locator('[data-k=c]')).toHaveText('899');
    await expect(maze(page).locator('.solve-maze :is(.entrance, .exit)')).toHaveCount(2);
    await expect(page.getByRole('button', { name: 'Finish maze' })).toBeDisabled();
    expect(await counter(page, 'v')).toBe(0);
    await expect(maze(page).locator('.status')).toHaveText('PAUSED');
    await page.getByRole('button', { name: 'Finish run' }).click();
    await expect(maze(page).locator('.status')).toHaveText(/^(✓ PATH FOUND|✕ FAILED)$/);
    await page.getByRole('button', { name: 'Run another' }).click();
    await expect(page.getByRole('button', { name: 'Finish maze' })).toBeDisabled();
  });

  test('when the API is unreachable the run says so instead of solving', async ({ page }) => {
    await page.route('**/api/maze**', (route) => route.abort());
    await page.goto('/');
    await inspect(page);
    await mazeHex(page).click();
    await page.getByRole('button', { name: 'Finish run' }).click();
    await expect(maze(page).locator('.status')).toHaveText('✕ API UNREACHABLE');
    await expect(maze(page).locator('.info-src')).toHaveText('The .NET API is unreachable.');
  });

  test('Race tab races five solvers on one maze and marks the guess right only for the winner', async ({ page }) => {
    await page.goto('/');
    await inspect(page);
    await mazeHex(page).click();
    await page.getByRole('button', { name: 'Race', exact: true }).click();
    const picks = maze(page).locator('.race-picks button');
    await expect(picks).toHaveCount(5);
    await expect(maze(page).locator('.race-maze :is(.entrance, .exit)')).toHaveCount(2);
    await expect(maze(page).locator('.solve-maze')).toBeHidden();

    await picks.first().click();
    await expect(maze(page).locator('.race-status')).toHaveText(/^(✓ You guessed it! |✕ ).+ (won|tied) in \d+ steps\.$/);
    expect(await maze(page).locator('.race-picks button[data-won]').count()).toBeGreaterThan(0);
    const guessedWinner = (await picks.first().getAttribute('data-won')) !== null;
    await expect(picks.first()).toHaveAttribute('data-verdict', guessedWinner ? 'right' : 'wrong');
    await expect(maze(page).locator('.race-maze .route').first()).toBeVisible();
    await expect(maze(page).locator('.race-maze .mine')).toHaveCount(1);

    await page.getByRole('button', { name: 'Solve', exact: true }).click();
    await expect(maze(page).locator('.solve-maze')).toBeVisible();
    await expect(maze(page).locator('.maze-race')).toBeHidden();
  });

  test('Tab reaches both module hexes; Esc returns focus to the maze hex', async ({ page }) => {
    await page.goto('/');
    await inspect(page);

    for (let i = 0; i < 6 && !(await mazeHex(page).evaluate((element) => element === document.activeElement)); i++) {
      await page.keyboard.press('Tab');
    }

    await expect(mazeHex(page)).toBeFocused();
    await page.keyboard.press('Enter');
    await expect(page.locator('dialog.open-hex')).toHaveAttribute('aria-label', 'Maze');
    await page.keyboard.press('Escape');
    await expect(mazeHex(page)).toBeFocused();
  });

  test('maze text is readable (>= 12 px, >= 4.5:1) in the cell and open views', async ({ page }) => {
    await page.goto('/');
    await inspect(page);
    const box = (await mazeHex(page).boundingBox())!;
    await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
    await expect(mazeHex(page)).toHaveClass(/active/);
    expect(await textProblems(page)).toEqual([]);
    await mazeHex(page).click();
    expect(await textProblems(page)).toEqual([]);
  });
});

test('Speed buttons change the playback speed between 0.25x and 8x', async ({ page }) => {
  await page.goto('/');
  await inspect(page);
  await sortHex(page).click();
  const speed = page.locator('dialog .mod-sort .speed');
  await expect(speed.locator('output')).toHaveText('1×');

  for (let i = 0; i < 3; i++) {
    await speed.getByRole('button', { name: 'Faster' }).click();
  }

  await expect(speed.locator('output')).toHaveText('8×');
  await expect(speed.getByRole('button', { name: 'Faster' })).toBeDisabled();

  for (let i = 0; i < 5; i++) {
    await speed.getByRole('button', { name: 'Slower' }).click();
  }

  await expect(speed.locator('output')).toHaveText('0.25×');
  await expect(speed.getByRole('button', { name: 'Slower' })).toBeDisabled();
});

const modules = [
  { name: 'Sorting Engine', hex: sortHex },
  { name: 'Maze', hex: mazeHex },
];

for (const { name, hex } of modules) {
  for (const width of [1440, 390]) {
    test(`open ${name} fits inside the hexagon at ${width}px`, async ({ page }) => {
      await page.setViewportSize({ width, height: 900 });
      await page.goto('/');
      await inspect(page);
      await hex(page).click();
      await expect(page.locator('dialog.open-hex')).toBeVisible();
      await expect.poll(() => page.locator('dialog.open-hex').evaluate((dialog) => {
        const hexagon = dialog.querySelector('.hex.big')!.getBoundingClientRect();

        const inside = (x: number, y: number) => {
          const across = (x - hexagon.left) / hexagon.width;
          const down = (y - hexagon.top) / hexagon.height;
          const edge = 0.25 * Math.abs(1 - 2 * across);

          return across >= 0 && across <= 1 && down >= edge && down <= 1 - edge;
        };

        const sticksOut = (element: Element) => {
          const box = element.getBoundingClientRect();

          if (!box.width || !box.height) {
            return false;
          }

          return !(inside(box.left, box.top) && inside(box.right, box.top) && inside(box.left, box.bottom) && inside(box.right, box.bottom));
        };

        return [...dialog.querySelectorAll('.mod[data-open] > *, .mod[data-open] .mod-head > *')]
          .filter(sticksOut)
          .map((element) => element.className || element.tagName);
      })).toEqual([]);
    });
  }
}

test.describe('Light bulb', () => {
  test('stays hidden until Ctrl+Alt+Shift+O', async ({ page }) => {
    await page.goto('/');
    await page.keyboard.press('Meta+Alt+Shift+KeyO');
    await expect(page.locator('.bulb-btn')).toBeHidden();
    await page.keyboard.press('Control+Alt+Shift+KeyO');
    await expect(page.locator('.bulb-btn')).toBeVisible();
  });

  test('lights the honeycomb after the signal and turns it off again', async ({ page }) => {
    await page.goto('/');
    await page.keyboard.press('Control+Alt+Shift+KeyO');
    await page.locator('.bulb-btn').click();
    await expect(page.locator('.bulb-btn')).toHaveAttribute('aria-pressed', 'true');
    await expect(page.locator('.signals')).toHaveCount(1);
    await expect(page.locator('.comb')).toHaveClass(/lit/);
    await page.locator('.bulb-btn').click();
    await expect(page.locator('.comb')).not.toHaveClass(/lit/);
  });

  test('inspect and the light switch each other off', async ({ page }) => {
    await page.goto('/');
    await page.keyboard.press('Control+Alt+Shift+KeyO');
    await inspect(page);
    await page.locator('.bulb-btn').click();
    await expect(page.locator('.inspect-btn')).toHaveAttribute('aria-pressed', 'false');
    await expect(page.locator('.bulb-btn')).toHaveAttribute('aria-pressed', 'true');
    await inspect(page);
    await expect(page.locator('.inspect-btn')).toHaveAttribute('aria-pressed', 'true');
    await expect(page.locator('.bulb-btn')).toHaveAttribute('aria-pressed', 'false');
    await expect(page.locator('.comb')).not.toHaveClass(/lit/);
  });

  test('with the light on every module opens by click or keyboard', async ({ page }) => {
    await page.goto('/');
    await page.keyboard.press('Control+Alt+Shift+KeyO');
    await page.locator('.bulb-btn').click();
    await sortHex(page).click();
    await expect(page.locator('.open-hex')).toBeVisible();
    await page.keyboard.press('Escape');
    await expect(page.locator('.open-hex')).toBeHidden();
    await expect(page.locator('.bulb-btn')).toHaveAttribute('aria-pressed', 'true');
    await mazeHex(page).focus();
    await page.keyboard.press('Enter');
    await expect(page.locator('.open-hex')).toBeVisible();
  });

  test('signing out with the light on leaves no hex marked as failed', async ({ page }) => {
    await page.goto('/');
    await page.keyboard.press('Control+Alt+Shift+KeyO');
    await page.locator('.bulb-btn').click();
    await page.waitForTimeout(1500);
    await page.locator('.hex[aria-label="Open Login"]').click();
    await page.getByRole('button', { name: 'Sign out' }).click();
    await page.waitForTimeout(3000);
    const backgrounds = await page.locator('.hex').evaluateAll((hexes) => hexes.map((hex) => getComputedStyle(hex).backgroundColor));
    expect(backgrounds).not.toContain('rgb(154, 92, 92)');
  });

  test('appears on the 50th click anywhere and hides again after reload', async ({ page }) => {
    await page.goto('/');
    for (let i = 0; i < 49; i++) {
      await page.mouse.click(10, 10);
    }

    await expect(page.locator('.bulb-btn')).toBeHidden();
    await page.mouse.click(10, 10);
    await expect(page.locator('.bulb-btn')).toBeVisible();
    await page.reload();
    await expect(page.locator('.bulb-btn')).toBeHidden();
  });
});
