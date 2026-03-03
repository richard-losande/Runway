import { createApp } from 'vue'
import { createPinia } from 'pinia'
import SproutDesignSystem from 'design-system-next'
import './style.css'
import App from './App.vue'

const app = createApp(App)
app.use(createPinia())
app.use(SproutDesignSystem)
app.mount('#app')
