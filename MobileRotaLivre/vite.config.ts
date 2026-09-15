import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
  // ESSA É A LINHA MÁGICA PARA O CLOUDFLARE:
  base: '/',
  
  plugins: [react(), tailwindcss()],

  server: {
    port: 3000,
    host: true,
    hmr: false,

    // O proxy funciona apenas na sua máquina (localhost).
    // No Cloudflare, certifique-se de que o React faz chamadas 
    // com o link inteiro "https://rotalivre-web.onrender.com/api/..."
    proxy: {
      '/api': {
        target: 'https://rotalivre-web.onrender.com',
        changeOrigin: true,
        secure: true
      }
    }
  }
});