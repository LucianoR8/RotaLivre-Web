import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
  plugins: [react(), tailwindcss()],

  server: {
    port: 3000,
    host: true,
    hmr: false,

    proxy: {
      '/api': {
        target: 'https://rotalivre-web.onrender.com',
        changeOrigin: true,
        secure: true
      }
    }
  }
});
