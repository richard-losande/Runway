import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'

const apiTarget =
  process.env.services__apiservice__https__0 ||
  process.env.services__apiservice__http__0 ||
  'http://localhost:5407'

export default defineConfig({
  plugins: [vue(), tailwindcss()],
  server: {
    port: parseInt(process.env.PORT || '5173'),
    proxy: {
      '/api': {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
