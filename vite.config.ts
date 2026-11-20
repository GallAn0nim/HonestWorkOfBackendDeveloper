import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    rollupOptions: {
      input: ['index.html', 'privacy.html'],
    },
  },
  server: {
    allowedHosts: ['.trycloudflare.com'],
    proxy: { '/api': 'http://localhost:5280' },
  },
});
