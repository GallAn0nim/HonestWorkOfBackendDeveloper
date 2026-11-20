import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: 'e2e',
  testMatch: '*.e2e.ts',
  use: { baseURL: 'http://localhost:5180', channel: 'chrome', viewport: { width: 1440, height: 900 } },
  webServer: [
    { command: 'npx vite --port 5180 --strictPort', url: 'http://localhost:5180', reuseExistingServer: false },
    {
      command: 'dotnet run --project api/HonestWork.Api',
      port: 5280,
      reuseExistingServer: true,
      timeout: 120_000,
      env: {
        RateLimiting__General__PermitLimit: '10000',
        RateLimiting__Auth__PermitLimit: '1000',
        RateLimiting__Heavy__PermitLimit: '1000',
      },
    },
  ],
});
