# Vue.js Login + Dashboard Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Replace the Blazor Hackathon.Web project with a Vue 3 + TypeScript SPA that authenticates via Keycloak (subdomain-based tenancy) and provides Dashboard + Financial Runway analyzer pages using sprout-design-system components.

**Architecture:** Vue app extracts tenant from subdomain, uses keycloak-js for OIDC redirect login, stores JWT token in Pinia, attaches it via Axios interceptor to all BFF requests. BFF already validates JWT and proxies to ApiService.

**Tech Stack:** Vue 3, TypeScript, Vite, keycloak-js, design-system-next (sprout-design-system), Vue Router, Pinia, Axios, Tailwind CSS

---

### Task 1: Scaffold Vue 3 + TypeScript project

**Files:**
- Delete all contents of: `Hackathon.Web/` (except keep the folder)
- Create: `Hackathon.Web/package.json`
- Create: `Hackathon.Web/index.html`
- Create: `Hackathon.Web/vite.config.ts`
- Create: `Hackathon.Web/tsconfig.json`
- Create: `Hackathon.Web/tsconfig.app.json`
- Create: `Hackathon.Web/env.d.ts`
- Create: `Hackathon.Web/.env.development`
- Create: `Hackathon.Web/src/main.ts`
- Create: `Hackathon.Web/src/App.vue`

**Step 1: Delete Blazor contents**

Remove everything inside `Hackathon.Web/` — all `.cs`, `.csproj`, `.razor` files, `Components/`, `Properties/`, `wwwroot/`, `bin/`, `obj/` folders:

```bash
cd Hackathon.Web
rm -rf Components Properties wwwroot bin obj
rm -f Program.cs WeatherApiClient.cs appsettings.json appsettings.Development.json Hackathon.Web.csproj Hackathon.Web.csproj.user
```

**Step 2: Create package.json**

Create `Hackathon.Web/package.json`:

```json
{
  "name": "hackathon-web",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vue-tsc && vite build",
    "preview": "vite preview"
  },
  "dependencies": {
    "vue": "^3.5.0",
    "vue-router": "^4.5.0",
    "pinia": "^3.0.0",
    "axios": "^1.7.0",
    "keycloak-js": "^26.0.0",
    "design-system-next": "^2.27.9"
  },
  "devDependencies": {
    "@vitejs/plugin-vue": "^5.2.0",
    "typescript": "~5.7.0",
    "vite": "^6.0.0",
    "vue-tsc": "^2.2.0",
    "tailwindcss": "^3.4.0",
    "postcss": "^8.4.0",
    "autoprefixer": "^10.4.0"
  }
}
```

**Step 3: Create index.html**

Create `Hackathon.Web/index.html`:

```html
<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <link rel="icon" type="image/svg+xml" href="/vite.svg" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Financial Runway Simulator</title>
  </head>
  <body>
    <div id="app"></div>
    <script type="module" src="/src/main.ts"></script>
  </body>
</html>
```

**Step 4: Create vite.config.ts**

Create `Hackathon.Web/vite.config.ts`:

```ts
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    port: 5173,
    host: true
  }
})
```

**Step 5: Create tsconfig files**

Create `Hackathon.Web/tsconfig.json`:

```json
{
  "files": [],
  "references": [
    { "path": "./tsconfig.app.json" }
  ]
}
```

Create `Hackathon.Web/tsconfig.app.json`:

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "module": "ESNext",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "isolatedModules": true,
    "moduleDetection": "force",
    "noEmit": true,
    "jsx": "preserve",
    "strict": true,
    "noUnusedLocals": false,
    "noUnusedParameters": false,
    "noFallthroughCasesInSwitch": true,
    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": ["src/**/*.ts", "src/**/*.tsx", "src/**/*.vue", "env.d.ts"]
}
```

Create `Hackathon.Web/env.d.ts`:

```ts
/// <reference types="vite/client" />

declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<{}, {}, any>
  export default component
}

declare module 'design-system-next'

interface ImportMetaEnv {
  readonly VITE_BFF_URL: string
  readonly VITE_KEYCLOAK_URL: string
  readonly VITE_KEYCLOAK_CLIENT_ID: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
```

**Step 6: Create .env.development**

Create `Hackathon.Web/.env.development`:

```env
VITE_BFF_URL=http://localhost:5208
VITE_KEYCLOAK_URL=https://sso-test.sprout.ph
VITE_KEYCLOAK_CLIENT_ID=account
```

**Step 7: Create Tailwind config**

Create `Hackathon.Web/tailwind.config.js`:

```js
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './index.html',
    './src/**/*.{vue,js,ts,jsx,tsx}',
    './node_modules/design-system-next/**/*.{vue,js,ts}'
  ],
  theme: {
    extend: {}
  },
  plugins: []
}
```

Create `Hackathon.Web/postcss.config.js`:

```js
export default {
  plugins: {
    tailwindcss: {},
    autoprefixer: {}
  }
}
```

**Step 8: Create minimal main.ts and App.vue**

Create `Hackathon.Web/src/main.ts`:

```ts
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import SproutDesignSystem from 'design-system-next'
import 'design-system-next/style.css'
import App from './App.vue'

const app = createApp(App)
app.use(createPinia())
app.use(SproutDesignSystem)
app.mount('#app')
```

Create `Hackathon.Web/src/App.vue`:

```vue
<template>
  <div id="app">
    <h1>Hackathon Web</h1>
    <spr-button tone="success">It works!</spr-button>
  </div>
</template>

<script setup lang="ts">
</script>
```

**Step 9: Install dependencies and verify**

```bash
cd Hackathon.Web
npm install
npm run dev
```

Expected: Vite starts on `http://localhost:5173`, page shows "Hackathon Web" heading and a green sprout-design-system button.

**Step 10: Commit**

```bash
git add Hackathon.Web/
git commit -m "feat(web): scaffold Vue 3 + TypeScript project with sprout-design-system"
```

---

### Task 2: TypeScript interfaces + API service

**Files:**
- Create: `Hackathon.Web/src/types/index.ts`
- Create: `Hackathon.Web/src/services/api.ts`

**Step 1: Create TypeScript interfaces**

Create `Hackathon.Web/src/types/index.ts`:

```ts
export interface LifeEventInput {
  type: string
  description: string
  monthFromNow: number
  recurring: boolean
  estimatedCost: number | null
}

export interface CategorizedExpense {
  category: string
  monthlyAverage: number
  percentage: number
}

export interface MonthlyProjection {
  month: number
  balance: number
  income: number
  expenses: number
}

export interface LifeEventImpact {
  event: string
  impactOnRunway: number
  newMonthlyExpense: number
}

export interface FinancialRunwayResponse {
  runwayMonths: number
  monthlyBurnRate: number
  categorizedExpenses: CategorizedExpense[]
  monthlyProjections: MonthlyProjection[]
  lifeEventImpacts: LifeEventImpact[]
  adjustedRunwayMonths: number
  narrative: string
  analyzedAt: string
}

export const LIFE_EVENT_TYPES = [
  { text: 'Buy House', value: 'BuyHouse' },
  { text: 'Buy Car', value: 'BuyCar' },
  { text: 'Have Baby', value: 'HaveBaby' },
  { text: 'Lose Job', value: 'LoseJob' },
  { text: 'Get Raise', value: 'GetRaise' },
  { text: 'Custom', value: 'Custom' }
]
```

**Step 2: Create Axios API service**

Create `Hackathon.Web/src/services/api.ts`:

```ts
import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_BFF_URL,
  timeout: 300000 // 5 minutes for OpenAI calls
})

export function setAuthToken(token: string | undefined) {
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`
  } else {
    delete api.defaults.headers.common['Authorization']
  }
}

export async function analyzeFinancialRunway(
  csvFile: File,
  monthlySalary: number,
  totalSavings: number,
  lifeEventsJson: string | null
) {
  const formData = new FormData()
  formData.append('csvFile', csvFile)
  formData.append('monthlySalary', monthlySalary.toString())
  formData.append('totalSavings', totalSavings.toString())
  if (lifeEventsJson) {
    formData.append('lifeEventsJson', lifeEventsJson)
  }

  const response = await api.post('/api/v1/financial-runway/analyze', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
  return response.data
}

export default api
```

**Step 3: Commit**

```bash
git add Hackathon.Web/src/types/ Hackathon.Web/src/services/
git commit -m "feat(web): add TypeScript interfaces and Axios API service"
```

---

### Task 3: Keycloak auth composable + Pinia store

**Files:**
- Create: `Hackathon.Web/src/composables/useKeycloak.ts`
- Create: `Hackathon.Web/src/stores/auth.ts`

**Step 1: Create Keycloak composable**

Create `Hackathon.Web/src/composables/useKeycloak.ts`:

```ts
import Keycloak from 'keycloak-js'

let keycloakInstance: Keycloak | null = null

export function getTenantFromHostname(): string {
  const hostname = window.location.hostname
  const parts = hostname.split('.')
  // e.g., "acme.localhost" -> "acme", "acme.app.com" -> "acme"
  // fallback to "master" if no subdomain (e.g., plain "localhost")
  if (parts.length >= 2 && parts[0] !== 'localhost' && parts[0] !== 'www') {
    return parts[0]
  }
  return 'master'
}

export function createKeycloak(): Keycloak {
  if (keycloakInstance) return keycloakInstance

  const tenant = getTenantFromHostname()

  keycloakInstance = new Keycloak({
    url: import.meta.env.VITE_KEYCLOAK_URL,
    realm: tenant,
    clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID
  })

  return keycloakInstance
}

export function getKeycloak(): Keycloak | null {
  return keycloakInstance
}
```

**Step 2: Create Pinia auth store**

Create `Hackathon.Web/src/stores/auth.ts`:

```ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { createKeycloak, getTenantFromHostname } from '@/composables/useKeycloak'
import { setAuthToken } from '@/services/api'
import type Keycloak from 'keycloak-js'

export const useAuthStore = defineStore('auth', () => {
  const keycloak = ref<Keycloak | null>(null)
  const initialized = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => keycloak.value?.authenticated ?? false)
  const token = computed(() => keycloak.value?.token)
  const username = computed(() => keycloak.value?.tokenParsed?.preferred_username ?? '')
  const tenant = computed(() => getTenantFromHostname())

  async function init() {
    try {
      const kc = createKeycloak()
      keycloak.value = kc

      const authenticated = await kc.init({
        onLoad: 'login-required',
        checkLoginIframe: false,
        pkceMethod: 'S256'
      })

      if (authenticated && kc.token) {
        setAuthToken(kc.token)
      }

      // Auto-refresh token before it expires
      kc.onTokenExpired = async () => {
        try {
          await kc.updateToken(30)
          setAuthToken(kc.token)
        } catch {
          await kc.login()
        }
      }

      initialized.value = true
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to initialize authentication'
      initialized.value = true
    }
  }

  function login() {
    keycloak.value?.login()
  }

  function logout() {
    keycloak.value?.logout({ redirectUri: window.location.origin })
  }

  return {
    keycloak,
    initialized,
    error,
    isAuthenticated,
    token,
    username,
    tenant,
    init,
    login,
    logout
  }
})
```

**Step 3: Commit**

```bash
git add Hackathon.Web/src/composables/ Hackathon.Web/src/stores/
git commit -m "feat(web): add Keycloak composable and Pinia auth store"
```

---

### Task 4: Vue Router with auth guard

**Files:**
- Create: `Hackathon.Web/src/router/index.ts`
- Create: `Hackathon.Web/src/pages/LoginPage.vue`
- Modify: `Hackathon.Web/src/main.ts`
- Modify: `Hackathon.Web/src/App.vue`

**Step 1: Create router**

Create `Hackathon.Web/src/router/index.ts`:

```ts
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/pages/LoginPage.vue')
    },
    {
      path: '/',
      name: 'dashboard',
      component: () => import('@/pages/DashboardPage.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/financial-runway',
      name: 'financial-runway',
      component: () => import('@/pages/FinancialRunwayPage.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

export default router
```

**Step 2: Create LoginPage placeholder**

Create `Hackathon.Web/src/pages/LoginPage.vue`:

```vue
<template>
  <div class="flex items-center justify-center min-h-screen spr-background-color">
    <spr-card title="Financial Runway Simulator">
      <template #content>
        <div class="flex flex-col items-center gap-4 p-6">
          <p v-if="auth.error" class="spr-text-color-danger-base">
            {{ auth.error }}
          </p>
          <p v-else class="spr-text-color-base">
            Redirecting to login...
          </p>
          <spr-button
            v-if="auth.error"
            tone="success"
            @click="auth.login()"
          >
            Try Again
          </spr-button>
        </div>
      </template>
    </spr-card>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
</script>
```

**Step 3: Create DashboardPage placeholder**

Create `Hackathon.Web/src/pages/DashboardPage.vue`:

```vue
<template>
  <div class="p-6">
    <h1 class="spr-heading-md mb-4">Dashboard</h1>
    <p>Welcome, {{ auth.username }}! (Tenant: {{ auth.tenant }})</p>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
</script>
```

**Step 4: Create FinancialRunwayPage placeholder**

Create `Hackathon.Web/src/pages/FinancialRunwayPage.vue`:

```vue
<template>
  <div class="p-6">
    <h1 class="spr-heading-md mb-4">Financial Runway Analyzer</h1>
    <p>Coming soon...</p>
  </div>
</template>

<script setup lang="ts">
</script>
```

**Step 5: Update main.ts to use router and init auth**

Replace `Hackathon.Web/src/main.ts` with:

```ts
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import SproutDesignSystem from 'design-system-next'
import 'design-system-next/style.css'
import App from './App.vue'
import router from './router'
import { useAuthStore } from './stores/auth'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(SproutDesignSystem)
app.use(router)

// Initialize Keycloak before mounting
const auth = useAuthStore()
auth.init().then(() => {
  app.mount('#app')
})
```

**Step 6: Update App.vue with router-view**

Replace `Hackathon.Web/src/App.vue` with:

```vue
<template>
  <div v-if="!auth.initialized" class="flex items-center justify-center min-h-screen">
    <p class="spr-text-color-base">Loading...</p>
  </div>
  <router-view v-else />
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
</script>
```

**Step 7: Verify**

```bash
cd Hackathon.Web
npm run dev
```

Expected: Opening `http://localhost:5173` should attempt Keycloak init (will fail if no Keycloak realm matches, but the app should load without JS errors). Check browser console for keycloak-js init attempts.

**Step 8: Commit**

```bash
git add Hackathon.Web/src/
git commit -m "feat(web): add Vue Router, auth guard, login and placeholder pages"
```

---

### Task 5: App layout with SprSidenav

**Files:**
- Create: `Hackathon.Web/src/components/AppLayout.vue`
- Modify: `Hackathon.Web/src/App.vue`

**Step 1: Create AppLayout component**

Create `Hackathon.Web/src/components/AppLayout.vue`:

```vue
<template>
  <div class="flex min-h-screen">
    <!-- Sidebar -->
    <aside class="w-60 spr-bg-kangkong-800 spr-text-white-50 flex flex-col">
      <div class="p-4 border-b border-white/10">
        <h2 class="spr-heading-sm spr-text-white-50">Financial Runway</h2>
        <p class="spr-body-sm-regular spr-text-kangkong-200 mt-1">{{ auth.tenant }}</p>
      </div>

      <nav class="flex-1 p-2 mt-2">
        <router-link
          v-for="link in navLinks"
          :key="link.path"
          :to="link.path"
          class="flex items-center gap-3 px-4 py-3 rounded-lg mb-1 transition-colors"
          :class="route.path === link.path
            ? 'spr-bg-kangkong-600 spr-text-white-50'
            : 'spr-text-kangkong-200 hover:spr-bg-kangkong-700 hover:spr-text-white-50'"
        >
          <spr-icon :id="link.icon + '-nav'" :icon="link.icon" size="md" />
          <span>{{ link.label }}</span>
        </router-link>
      </nav>

      <div class="p-4 border-t border-white/10">
        <div class="flex items-center gap-3 mb-3">
          <spr-avatar variant="initial" :initial="initials" size="sm" />
          <div>
            <p class="spr-body-sm-bold spr-text-white-50">{{ auth.username }}</p>
          </div>
        </div>
        <spr-button variant="secondary" size="small" fullwidth @click="auth.logout()">
          Logout
        </spr-button>
      </div>
    </aside>

    <!-- Main content -->
    <main class="flex-1 spr-background-color overflow-auto">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const auth = useAuthStore()

const initials = computed(() => {
  const name = auth.username
  return name ? name.substring(0, 2).toUpperCase() : '?'
})

const navLinks = [
  { path: '/', label: 'Dashboard', icon: 'ph:house-simple' },
  { path: '/financial-runway', label: 'Financial Runway', icon: 'ph:chart-line-up' }
]
</script>
```

**Step 2: Update App.vue to use layout**

Replace `Hackathon.Web/src/App.vue` with:

```vue
<template>
  <div v-if="!auth.initialized" class="flex items-center justify-center min-h-screen">
    <p class="spr-text-color-base">Loading...</p>
  </div>
  <template v-else>
    <AppLayout v-if="auth.isAuthenticated" />
    <router-view v-else />
  </template>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import AppLayout from '@/components/AppLayout.vue'

const auth = useAuthStore()
</script>
```

**Step 3: Commit**

```bash
git add Hackathon.Web/src/
git commit -m "feat(web): add AppLayout with sidebar navigation and logout"
```

---

### Task 6: Dashboard page

**Files:**
- Modify: `Hackathon.Web/src/pages/DashboardPage.vue`

**Step 1: Build the dashboard page**

Replace `Hackathon.Web/src/pages/DashboardPage.vue` with:

```vue
<template>
  <div class="p-6">
    <div class="mb-6">
      <h1 class="spr-heading-lg">Welcome back, {{ auth.username }}</h1>
      <p class="spr-body-md-regular spr-text-color-subtle mt-1">
        Tenant: {{ auth.tenant }}
      </p>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <!-- Quick action card -->
      <spr-card title="Financial Runway Analyzer">
        <template #content>
          <div class="p-4">
            <p class="spr-body-md-regular mb-4">
              Upload your bank statement CSV, enter your salary and savings,
              add life events, and get an AI-powered financial runway analysis.
            </p>
            <router-link to="/financial-runway">
              <spr-button tone="success">
                Start Analysis
              </spr-button>
            </router-link>
          </div>
        </template>
      </spr-card>

      <!-- Info card -->
      <spr-card title="How It Works">
        <template #content>
          <div class="p-4 space-y-3">
            <div class="flex items-start gap-3">
              <spr-icon id="upload-icon" icon="ph:upload-simple" size="md" />
              <p class="spr-body-sm-regular">Upload a bank statement CSV with your transactions</p>
            </div>
            <div class="flex items-start gap-3">
              <spr-icon id="money-icon" icon="ph:money" size="md" />
              <p class="spr-body-sm-regular">Enter your monthly salary and total savings</p>
            </div>
            <div class="flex items-start gap-3">
              <spr-icon id="calendar-icon" icon="ph:calendar" size="md" />
              <p class="spr-body-sm-regular">Add planned life events (car, house, baby, etc.)</p>
            </div>
            <div class="flex items-start gap-3">
              <spr-icon id="chart-icon" icon="ph:chart-bar" size="md" />
              <p class="spr-body-sm-regular">Get AI-powered analysis with runway projections</p>
            </div>
          </div>
        </template>
      </spr-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Web/src/pages/DashboardPage.vue
git commit -m "feat(web): build dashboard page with welcome and quick action cards"
```

---

### Task 7: Financial Runway form page

**Files:**
- Modify: `Hackathon.Web/src/pages/FinancialRunwayPage.vue`

**Step 1: Build the full form page**

Replace `Hackathon.Web/src/pages/FinancialRunwayPage.vue` with:

```vue
<template>
  <div class="p-6 max-w-4xl">
    <h1 class="spr-heading-lg mb-6">Financial Runway Analyzer</h1>

    <!-- Error banner -->
    <spr-banner
      v-model:show="showError"
      type="error"
      :message="errorMessage"
      class="mb-4"
    />

    <!-- Form -->
    <spr-card title="Upload & Configure">
      <template #content>
        <div class="p-4 space-y-6">

          <!-- CSV Upload -->
          <div>
            <label class="spr-body-sm-bold block mb-2">Bank Statement CSV</label>
            <spr-file-upload
              v-model="csvFiles"
              :file-types="['text/csv', '.csv']"
              :max-file-size="10"
            />
          </div>

          <!-- Salary & Savings -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <spr-input
              v-model="monthlySalary"
              label="Monthly Salary"
              placeholder="e.g. 5000"
              type="number"
            />
            <spr-input
              v-model="totalSavings"
              label="Total Savings"
              placeholder="e.g. 25000"
              type="number"
            />
          </div>

          <!-- Life Events -->
          <div>
            <div class="flex items-center justify-between mb-3">
              <label class="spr-body-sm-bold">Life Events</label>
              <spr-button size="small" variant="secondary" @click="addLifeEvent">
                + Add Event
              </spr-button>
            </div>

            <div
              v-for="(event, index) in lifeEvents"
              :key="index"
              class="border spr-border-color-base spr-rounded-border-radius-md p-4 mb-3"
            >
              <div class="flex justify-between items-start mb-3">
                <span class="spr-body-sm-bold">Event {{ index + 1 }}</span>
                <spr-button
                  size="small"
                  tone="danger"
                  variant="tertiary"
                  @click="removeLifeEvent(index)"
                >
                  Remove
                </spr-button>
              </div>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
                <spr-select
                  :id="'event-type-' + index"
                  v-model="event.type"
                  label="Event Type"
                  :options="lifeEventTypeOptions"
                  placeholder="Select type"
                />
                <spr-input
                  v-model="event.description"
                  label="Description"
                  placeholder="e.g. Used Toyota Corolla"
                />
                <spr-input
                  v-model="event.monthFromNow"
                  label="Months From Now"
                  placeholder="e.g. 3"
                  type="number"
                />
                <spr-input
                  v-model="event.estimatedCost"
                  label="Estimated Cost"
                  placeholder="e.g. 15000"
                  type="number"
                />
              </div>

              <div class="mt-3">
                <spr-checkbox
                  v-model="event.recurring"
                  label="Recurring monthly expense"
                  description="Check if this is a monthly cost (e.g. car payment, mortgage)"
                />
              </div>
            </div>
          </div>

          <!-- Submit -->
          <spr-button
            tone="success"
            fullwidth
            :disabled="isLoading || !csvFiles.length"
            @click="submitAnalysis"
          >
            {{ isLoading ? 'Analyzing... (this may take a minute)' : 'Analyze Financial Runway' }}
          </spr-button>
        </div>
      </template>
    </spr-card>

    <!-- Results -->
    <div v-if="result" class="mt-6 space-y-6">
      <FinancialResults :result="result" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { analyzeFinancialRunway } from '@/services/api'
import { LIFE_EVENT_TYPES } from '@/types'
import type { LifeEventInput, FinancialRunwayResponse } from '@/types'
import FinancialResults from '@/components/FinancialResults.vue'

const csvFiles = ref<File[]>([])
const monthlySalary = ref('')
const totalSavings = ref('')
const lifeEvents = reactive<LifeEventInput[]>([])
const isLoading = ref(false)
const result = ref<FinancialRunwayResponse | null>(null)
const showError = ref(false)
const errorMessage = ref('')

const lifeEventTypeOptions = LIFE_EVENT_TYPES

function addLifeEvent() {
  lifeEvents.push({
    type: '',
    description: '',
    monthFromNow: 0,
    recurring: false,
    estimatedCost: null
  })
}

function removeLifeEvent(index: number) {
  lifeEvents.splice(index, 1)
}

async function submitAnalysis() {
  if (!csvFiles.value.length) return

  isLoading.value = true
  showError.value = false
  result.value = null

  try {
    const lifeEventsJson = lifeEvents.length > 0
      ? JSON.stringify(lifeEvents)
      : null

    result.value = await analyzeFinancialRunway(
      csvFiles.value[0],
      Number(monthlySalary.value),
      Number(totalSavings.value),
      lifeEventsJson
    )
  } catch (e: any) {
    errorMessage.value = e.response?.data?.detail || e.message || 'Analysis failed'
    showError.value = true
  } finally {
    isLoading.value = false
  }
}
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Web/src/pages/FinancialRunwayPage.vue
git commit -m "feat(web): build Financial Runway form with CSV upload and life events"
```

---

### Task 8: Financial Results display component

**Files:**
- Create: `Hackathon.Web/src/components/FinancialResults.vue`

**Step 1: Create the results component**

Create `Hackathon.Web/src/components/FinancialResults.vue`:

```vue
<template>
  <div class="space-y-6">
    <!-- Success banner -->
    <spr-banner
      :show="true"
      type="success"
      message="Analysis complete!"
    />

    <!-- Runway Summary -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <spr-card title="Runway">
        <template #content>
          <div class="p-4 text-center">
            <p class="spr-heading-xl spr-text-color-brand-base">
              {{ result.runwayMonths }}
            </p>
            <p class="spr-body-sm-regular spr-text-color-subtle">months (base)</p>
          </div>
        </template>
      </spr-card>

      <spr-card title="Adjusted Runway">
        <template #content>
          <div class="p-4 text-center">
            <p class="spr-heading-xl" :class="result.adjustedRunwayMonths < 6 ? 'spr-text-tomato-500' : 'spr-text-color-brand-base'">
              {{ result.adjustedRunwayMonths }}
            </p>
            <p class="spr-body-sm-regular spr-text-color-subtle">months (with events)</p>
          </div>
        </template>
      </spr-card>

      <spr-card title="Monthly Burn Rate">
        <template #content>
          <div class="p-4 text-center">
            <p class="spr-heading-xl spr-text-color-base">
              ${{ result.monthlyBurnRate.toLocaleString() }}
            </p>
            <p class="spr-body-sm-regular spr-text-color-subtle">per month</p>
          </div>
        </template>
      </spr-card>
    </div>

    <!-- Categorized Expenses -->
    <spr-card title="Expense Breakdown">
      <template #content>
        <div class="p-4">
          <spr-table
            :headers="expenseHeaders"
            :data-table="expenseData"
          />
        </div>
      </template>
    </spr-card>

    <!-- Monthly Projections -->
    <spr-card title="Monthly Projections">
      <template #content>
        <div class="p-4">
          <spr-table
            :headers="projectionHeaders"
            :data-table="projectionData"
          />
        </div>
      </template>
    </spr-card>

    <!-- Life Event Impacts -->
    <spr-card v-if="result.lifeEventImpacts.length" title="Life Event Impacts">
      <template #content>
        <div class="p-4">
          <spr-table
            :headers="impactHeaders"
            :data-table="impactData"
          />
        </div>
      </template>
    </spr-card>

    <!-- Narrative -->
    <spr-card title="AI Analysis & Advice">
      <template #content>
        <div class="p-4">
          <p class="spr-body-md-regular whitespace-pre-line">{{ result.narrative }}</p>
        </div>
      </template>
    </spr-card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { FinancialRunwayResponse } from '@/types'

const props = defineProps<{
  result: FinancialRunwayResponse
}>()

const expenseHeaders = [
  { field: 'category', name: 'Category' },
  { field: 'monthlyAverage', name: 'Monthly Average' },
  { field: 'percentage', name: 'Percentage' }
]

const expenseData = computed(() =>
  props.result.categorizedExpenses.map(e => ({
    category: e.category,
    monthlyAverage: `$${e.monthlyAverage.toLocaleString()}`,
    percentage: `${e.percentage.toFixed(1)}%`
  }))
)

const projectionHeaders = [
  { field: 'month', name: 'Month' },
  { field: 'balance', name: 'Balance' },
  { field: 'income', name: 'Income' },
  { field: 'expenses', name: 'Expenses' }
]

const projectionData = computed(() =>
  props.result.monthlyProjections.map(p => ({
    month: `Month ${p.month}`,
    balance: `$${p.balance.toLocaleString()}`,
    income: `$${p.income.toLocaleString()}`,
    expenses: `$${p.expenses.toLocaleString()}`
  }))
)

const impactHeaders = [
  { field: 'event', name: 'Event' },
  { field: 'impactOnRunway', name: 'Impact (months)' },
  { field: 'newMonthlyExpense', name: 'New Monthly Cost' }
]

const impactData = computed(() =>
  props.result.lifeEventImpacts.map(i => ({
    event: i.event,
    impactOnRunway: i.impactOnRunway > 0 ? `+${i.impactOnRunway}` : `${i.impactOnRunway}`,
    newMonthlyExpense: `$${i.newMonthlyExpense.toLocaleString()}`
  }))
)
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Web/src/components/FinancialResults.vue
git commit -m "feat(web): add FinancialResults component with summary cards and tables"
```

---

### Task 9: Remove Hackathon.Web from slnx and update AppHost

**Files:**
- Modify: `Hackathon.slnx` (remove Hackathon.Web csproj reference if present)
- Verify: `Hackathon.AppHost/AppHost.cs` (already doesn't reference Web)

**Step 1: Check and clean slnx**

```bash
cat Hackathon.slnx
```

If `Hackathon.Web.csproj` is referenced, remove that line. The Web project is now a standalone Vue app, not a .NET project.

**Step 2: Verify AppHost doesn't reference Web**

Confirm `Hackathon.AppHost/AppHost.cs` has no reference to `Hackathon_Web`. It should only have `apiservice` and `bff` (which it already does).

**Step 3: Add .gitignore for node_modules**

Create or update `Hackathon.Web/.gitignore`:

```
node_modules/
dist/
*.local
```

**Step 4: Commit**

```bash
git add Hackathon.slnx Hackathon.Web/.gitignore
git commit -m "chore: remove Blazor Web project from solution, add node .gitignore"
```

---

### Task 10: Smoke test end-to-end

**Step 1: Start BFF + ApiService**

```bash
dotnet run --project Hackathon.AppHost
```

**Step 2: Start Vue dev server**

```bash
cd Hackathon.Web
npm run dev
```

**Step 3: Test with subdomain**

Open browser to `http://{your-realm}.localhost:5173`

Expected flow:
1. App loads, shows "Loading..."
2. keycloak-js initializes with the realm from subdomain
3. Redirects to Keycloak login page
4. After login, redirects back to dashboard
5. Sidebar shows with Dashboard and Financial Runway links
6. Navigate to Financial Runway, upload CSV, submit
7. Results display in cards and tables

**Step 4: If no Keycloak realm available for testing**

Temporarily modify `Hackathon.Web/src/stores/auth.ts` to bypass auth:
- Set `initialized.value = true` immediately
- Set a mock username
- Comment out the `kc.init()` call

This lets you test the UI without a live Keycloak instance.
