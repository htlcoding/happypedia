import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      // Alle /api-Anfragen an das ASP.NET-Backend weiterleiten.
      // So entfallen CORS-Probleme im Dev-Betrieb und das Frontend
      // kann einfach relative Pfade wie "/api/articles" nutzen.
      '/api': {
        target: 'http://localhost:5227',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
