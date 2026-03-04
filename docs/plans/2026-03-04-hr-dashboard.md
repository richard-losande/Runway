# HR Dashboard Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add a new `/dashboard` page to the Vue 3 frontend that replicates the Sprout HR dashboard Figma design (node 111:790) using Sprout Design System components and static mock data.

**Architecture:** Vue Router splits the app into two routes: `/` (existing financial runway flow, untouched) and `/dashboard` (new HR dashboard). The dashboard page is composed of sub-components in `src/views/dashboard/`. All Sprout DS components are already globally registered via the plugin in `main.ts`.

**Tech Stack:** Vue 3, Vue Router 4, Pinia, Tailwind CSS v4, design-system-next v2.27.9, TypeScript

---

### Task 1: Install vue-router and create router config

**Files:**
- Create: `Hackathon.Frontend/src/router.ts`
- Modify: `Hackathon.Frontend/package.json` (dependency added by npm)

**Step 1: Install vue-router**

Run:
```bash
cd Hackathon.Frontend && npm install vue-router@4
```

Expected: vue-router@4.x added to package.json dependencies

**Step 2: Create router.ts**

Create `Hackathon.Frontend/src/router.ts`:

```ts
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: () => import('./views/RunwayApp.vue'),
    },
    {
      path: '/dashboard',
      component: () => import('./views/dashboard/DashboardPage.vue'),
    },
  ],
})

export default router
```

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/router.ts Hackathon.Frontend/package.json Hackathon.Frontend/package-lock.json
git commit -m "feat: add vue-router with / and /dashboard routes"
```

---

### Task 2: Extract existing app into RunwayApp.vue and wire up router

**Files:**
- Create: `Hackathon.Frontend/src/views/RunwayApp.vue`
- Modify: `Hackathon.Frontend/src/App.vue`
- Modify: `Hackathon.Frontend/src/main.ts`

**Step 1: Create RunwayApp.vue**

Copy the entire current content of `App.vue` into `Hackathon.Frontend/src/views/RunwayApp.vue` (unchanged — same template, same script, same imports):

```vue
<template>
  <div class="min-h-screen bg-gray-50">
    <PayFeedScreen v-if="store.currentScreen === 1" />
    <PayrollProfile v-else-if="store.currentScreen === 2" />
    <DataConnection v-else-if="store.currentScreen === 3" />
    <ProcessingScreenV4 v-else-if="store.currentScreen === 4" />
    <IntelligenceReportV4 v-else-if="store.currentScreen === 5" />
    <SurvivalDashboardV4 v-else-if="store.currentScreen === 6" />
    <DiagnosisScreenV4 v-else-if="store.currentScreen === 7" />
    <ActionCard v-else-if="store.currentScreen === 8" />
  </div>
</template>

<script setup lang="ts">
import { useRunwayV4Store } from '../stores/runway-v4'
import PayFeedScreen from '../components/v4/PayFeedScreen.vue'
import PayrollProfile from '../components/v4/PayrollProfile.vue'
import DataConnection from '../components/v4/DataConnection.vue'
import ProcessingScreenV4 from '../components/v4/ProcessingScreenV4.vue'
import IntelligenceReportV4 from '../components/v4/IntelligenceReportV4.vue'
import SurvivalDashboardV4 from '../components/v4/SurvivalDashboardV4.vue'
import DiagnosisScreenV4 from '../components/v4/DiagnosisScreenV4.vue'
import ActionCard from '../components/v4/ActionCard.vue'

const store = useRunwayV4Store()
</script>
```

Note: Import paths change from `'./stores/...'` to `'../stores/...'` and `'./components/...'` to `'../components/...'` because the file moves one level deeper.

**Step 2: Replace App.vue with router-view shell**

Replace `Hackathon.Frontend/src/App.vue` with:

```vue
<template>
  <router-view />
</template>
```

**Step 3: Register router in main.ts**

Modify `Hackathon.Frontend/src/main.ts` to:

```ts
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import SproutDesignSystem from 'design-system-next'
import router from './router'
import './style.css'
import App from './App.vue'

const app = createApp(App)
app.use(createPinia())
app.use(router)
app.use(SproutDesignSystem)
app.mount('#app')
```

**Step 4: Verify existing app still works**

Run: `cd Hackathon.Frontend && npm run dev`

Open `http://localhost:5173/` — should show the same PayFeedScreen as before. No visual change.

**Step 5: Commit**

```bash
git add Hackathon.Frontend/src/views/RunwayApp.vue Hackathon.Frontend/src/App.vue Hackathon.Frontend/src/main.ts
git commit -m "feat: extract RunwayApp.vue and wire up vue-router"
```

---

### Task 3: Create DashboardPage.vue shell with sidebar layout

**Files:**
- Create: `Hackathon.Frontend/src/views/dashboard/DashboardPage.vue`
- Create: `Hackathon.Frontend/src/views/dashboard/DashboardSidebar.vue`

**Step 1: Create DashboardSidebar.vue**

Create `Hackathon.Frontend/src/views/dashboard/DashboardSidebar.vue`:

```vue
<template>
  <nav class="flex flex-col items-center bg-white border-r border-[#eff1f1] w-[68px] min-h-screen py-4 px-3 gap-2">
    <!-- Logo -->
    <div class="w-9 h-9 bg-[#00291b] rounded flex items-center justify-center mb-2">
      <span class="text-white text-xs font-bold">S</span>
    </div>

    <!-- Main nav icons -->
    <SidebarIcon icon="ph:magnifying-glass" label="Search" />
    <SidebarIcon icon="ph:star-four" label="AI" />
    <SidebarIcon icon="ph:house-simple-fill" label="Dashboard" :active="true" />
    <SidebarIcon icon="ph:chart-bar" label="Analytics" />
    <SidebarIcon icon="ph:squares-four" label="Apps" />

    <div class="w-9 h-[2px] bg-[#eff1f1] my-1" />

    <SidebarIcon icon="ph:money" label="Payroll" />
    <SidebarIcon icon="ph:wallet" label="Finances" />
    <SidebarIcon icon="ph:flow-arrow" label="Flow" />

    <div class="w-9 h-[2px] bg-[#eff1f1] my-1" />

    <SidebarIcon icon="ph:gear" label="Setup" />

    <div class="flex-1" />

    <!-- Bottom icons -->
    <SidebarIcon icon="ph:bell" label="Notifications" />
    <SidebarIcon icon="ph:file-text" label="Requests" />

    <!-- Profile avatar -->
    <div class="w-9 h-9 rounded-full bg-[#e1e6e4] flex items-center justify-center mt-2 relative">
      <span class="text-[#5a756b] text-xs font-medium">JD</span>
      <span class="absolute -top-1 -right-1 w-6 h-6 bg-[#e14942] rounded-full text-white text-xs flex items-center justify-center font-medium">16</span>
    </div>
  </nav>
</template>

<script setup lang="ts">
import SidebarIcon from './SidebarIcon.vue'
</script>
```

**Step 2: Create SidebarIcon.vue helper**

Create `Hackathon.Frontend/src/views/dashboard/SidebarIcon.vue`:

```vue
<template>
  <button
    class="w-9 h-9 rounded-lg flex items-center justify-center transition-colors"
    :class="active ? 'bg-[#dcfce6]' : 'hover:bg-[#f7f8f8]'"
    :title="label"
  >
    <Icon :icon="icon" class="text-xl text-[#262b2b]" />
  </button>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'

defineProps<{
  icon: string
  label: string
  active?: boolean
}>()
</script>
```

**Step 3: Create DashboardPage.vue**

Create `Hackathon.Frontend/src/views/dashboard/DashboardPage.vue`:

```vue
<template>
  <div class="flex min-h-screen bg-[#f0f2f2] font-['Rubik']">
    <DashboardSidebar />
    <main class="flex-1 flex flex-col bg-[#f7f8f8]">
      <AnnouncementBar />
      <div class="flex-1 bg-[#f1f2f3] px-20 py-6 flex flex-col gap-6 overflow-y-auto">
        <!-- Page Header -->
        <div class="flex items-center justify-between">
          <h1 class="text-xl font-medium text-black">Dashboard</h1>
          <button class="flex items-center gap-2 bg-white rounded-lg px-2 py-3 text-sm font-medium text-[#262b2b] border border-[#eff1f1]">
            <Icon icon="ph:wrench" class="text-base" />
            Customize Dashboard
          </button>
        </div>

        <ProfileCard />

        <!-- Two-column grid -->
        <div class="grid grid-cols-2 gap-4">
          <div class="flex flex-col gap-4">
            <TasksWidget />
            <LeaveWidget />
          </div>
          <PayslipsWidget />
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'
import DashboardSidebar from './DashboardSidebar.vue'
import AnnouncementBar from './AnnouncementBar.vue'
import ProfileCard from './ProfileCard.vue'
import TasksWidget from './TasksWidget.vue'
import PayslipsWidget from './PayslipsWidget.vue'
import LeaveWidget from './LeaveWidget.vue'
</script>
```

**Step 4: Create placeholder stubs for child components**

Create each of these files with a minimal placeholder so the page compiles:

`Hackathon.Frontend/src/views/dashboard/AnnouncementBar.vue`:
```vue
<template>
  <div class="px-6 py-4 bg-white border-b border-[#eff1f1]">
    <span class="text-sm text-[#3c5b51]">Announcement placeholder</span>
  </div>
</template>
```

`Hackathon.Frontend/src/views/dashboard/ProfileCard.vue`:
```vue
<template>
  <div class="bg-white rounded-2xl p-4">Profile card placeholder</div>
</template>
```

`Hackathon.Frontend/src/views/dashboard/TasksWidget.vue`:
```vue
<template>
  <div class="bg-white rounded-2xl p-4">Tasks widget placeholder</div>
</template>
```

`Hackathon.Frontend/src/views/dashboard/PayslipsWidget.vue`:
```vue
<template>
  <div class="bg-white rounded-2xl p-4">Payslips widget placeholder</div>
</template>
```

`Hackathon.Frontend/src/views/dashboard/LeaveWidget.vue`:
```vue
<template>
  <div class="bg-white rounded-2xl p-4">Leave widget placeholder</div>
</template>
```

**Step 5: Verify**

Run: `npm run dev`

Open `http://localhost:5173/dashboard` — should show the sidebar on the left and placeholder cards in the main area.

**Step 6: Commit**

```bash
git add Hackathon.Frontend/src/views/dashboard/
git commit -m "feat: add dashboard page shell with sidebar and placeholder widgets"
```

---

### Task 4: Build AnnouncementBar component

**Files:**
- Modify: `Hackathon.Frontend/src/views/dashboard/AnnouncementBar.vue`

**Step 1: Implement AnnouncementBar**

Replace `Hackathon.Frontend/src/views/dashboard/AnnouncementBar.vue` with:

```vue
<template>
  <div class="flex items-center justify-between px-6 py-4 bg-white border-b border-[#eff1f1]">
    <div class="flex items-center gap-2 min-w-0">
      <Icon icon="ph:megaphone-fill" class="text-[#17ad49] text-xl flex-shrink-0" />
      <span class="text-sm font-medium text-[#00291b] flex-shrink-0">Let's Celebrate, Happy Birthday!</span>
      <p class="text-sm text-[#3c5b51] truncate">
        Happy Birthday to these Rockstars! We hope your special day is filled with joy, laughter, and everything that makes you smile.
      </p>
    </div>
    <button class="flex items-center gap-1 bg-[#eff1f1] rounded-md px-1 py-1.5 text-xs font-medium text-[#262b2b] flex-shrink-0">
      <span class="px-1">see more</span>
      <Icon icon="ph:caret-right" class="text-xs" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'
</script>
```

**Step 2: Verify**

Open `http://localhost:5173/dashboard` — announcement bar should show green megaphone, birthday text, and "see more" button.

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/views/dashboard/AnnouncementBar.vue
git commit -m "feat: implement AnnouncementBar with birthday message"
```

---

### Task 5: Build ProfileCard component

**Files:**
- Modify: `Hackathon.Frontend/src/views/dashboard/ProfileCard.vue`

**Step 1: Implement ProfileCard**

Replace `Hackathon.Frontend/src/views/dashboard/ProfileCard.vue` with:

```vue
<template>
  <div class="bg-white rounded-2xl overflow-hidden">
    <!-- Decorative top border -->
    <div class="h-[18px] bg-[#14532b] relative overflow-hidden">
      <svg class="absolute inset-0 w-full h-full" viewBox="0 0 782 18" preserveAspectRatio="none">
        <path d="M0 18 Q200 -5 400 12 T782 0 V18 Z" fill="#22c558" opacity="0.6" />
        <path d="M0 18 Q150 0 350 15 T782 5 V18 Z" fill="#e0c651" opacity="0.4" />
        <path d="M0 18 Q250 5 500 10 T782 2 V18 Z" fill="#1ed5f2" opacity="0.3" />
      </svg>
    </div>

    <!-- Content -->
    <div class="flex items-center justify-between px-4 py-4 gap-4">
      <div class="flex items-center gap-4">
        <!-- Avatar -->
        <div class="w-14 h-14 rounded-full bg-[#e1e6e4] flex items-center justify-center relative">
          <span class="text-[#5a756b] text-lg font-medium">JD</span>
          <span class="absolute bottom-0 right-0 w-2.5 h-2.5 bg-[#989898] rounded-full border-2 border-white" />
        </div>

        <!-- Info -->
        <div class="flex flex-col">
          <span class="text-xl font-medium text-[#262b2b]">Hello, Jane 👋</span>
          <div class="flex items-center gap-2">
            <span class="text-base text-[#5d6c6b]">Expected at 09:00 AM</span>
            <span class="inline-flex items-center gap-1 bg-[#dcfce6] rounded-md px-1 py-0.5 text-xs font-medium text-[#158039]">
              <Icon icon="ph:clock" class="text-xs" />
              09:00 AM - 06:00 PM
            </span>
          </div>
        </div>
      </div>

      <!-- Clock buttons -->
      <div class="flex items-center gap-2">
        <button class="flex items-center gap-1 bg-[#158039] text-white rounded-lg px-2 py-2.5 text-sm font-medium">
          <Icon icon="ph:sign-in" class="text-base" />
          Clock In
        </button>
        <button class="flex items-center gap-1 bg-[#e6eaea] text-[#262b2b] rounded-lg px-2 py-2.5 text-sm font-medium">
          <Icon icon="ph:sign-out" class="text-base" />
          Clock Out
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'
</script>
```

**Step 2: Verify**

Open `http://localhost:5173/dashboard` — profile card shows avatar, greeting, shift times, and clock buttons.

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/views/dashboard/ProfileCard.vue
git commit -m "feat: implement ProfileCard with greeting and clock buttons"
```

---

### Task 6: Build TasksWidget component

**Files:**
- Modify: `Hackathon.Frontend/src/views/dashboard/TasksWidget.vue`

**Step 1: Implement TasksWidget**

Replace `Hackathon.Frontend/src/views/dashboard/TasksWidget.vue` with:

```vue
<template>
  <div class="bg-white rounded-2xl overflow-hidden">
    <!-- Header -->
    <div class="flex items-center justify-between px-4 py-3 border-b border-[#eff1f1]">
      <div class="flex items-center gap-2">
        <Icon icon="ph:check-square-fill" class="text-xl text-[#17ad49]" />
        <span class="text-base font-medium text-[#00291b]">My Tasks</span>
        <span class="bg-[#da2f38] text-white text-xs font-medium rounded-full w-5 h-5 flex items-center justify-center">5</span>
      </div>
      <div class="flex items-center gap-2">
        <button class="flex items-center gap-1 bg-white border border-[#eff1f1] rounded-lg px-2 py-1.5 text-xs font-medium text-[#262b2b]">
          <Icon icon="ph:arrows-down-up" class="text-xs" />
          Sort by
          <Icon icon="ph:caret-down" class="text-xs" />
        </button>
        <button class="bg-white border border-[#eff1f1] rounded-lg p-1.5">
          <Icon icon="ph:dots-three-vertical" class="text-sm text-[#262b2b]" />
        </button>
      </div>
    </div>

    <!-- Task rows -->
    <div class="divide-y divide-[#eff1f1]">
      <div
        v-for="task in tasks"
        :key="task.title"
        class="flex items-center justify-between px-4 py-2 hover:bg-[#f7f8f8] transition-colors"
      >
        <div class="flex items-center gap-2">
          <div class="w-7 h-7 bg-white border border-[#eff1f1] rounded flex items-center justify-center">
            <Icon :icon="task.icon" class="text-sm" :class="task.iconColor" />
          </div>
          <div class="flex flex-col">
            <span class="text-sm font-medium text-[#262b2b]">{{ task.title }}</span>
            <span class="text-sm text-[#5d6c6b]">{{ task.subtitle }}</span>
          </div>
        </div>
        <div class="flex items-center gap-4">
          <span class="text-sm text-[#5d6c6b]">{{ task.date }}</span>
          <span
            class="inline-flex items-center gap-1 rounded-md px-1 py-0.5 text-xs font-medium"
            :class="statusClasses[task.status]"
          >
            <span class="w-2 h-2 rounded-full" :class="statusDotClasses[task.status]" />
            {{ task.statusLabel }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'

const statusClasses: Record<string, string> = {
  resolution: 'bg-[#fff4d3] text-[#cc5c02]',
  action: 'bg-[#d8ebff] text-[#0f6eeb]',
}

const statusDotClasses: Record<string, string> = {
  resolution: 'bg-[#cc5c02]',
  action: 'bg-[#0f6eeb]',
}

const tasks = [
  {
    title: 'Missing Log',
    subtitle: 'System Generated',
    date: 'Jan 06, 2025',
    status: 'resolution',
    statusLabel: 'Needs resolution',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#cc5c02]',
  },
  {
    title: 'IT Acknowledgement Form',
    subtitle: 'Timothy Chalamat',
    date: 'Jan 03, 2025',
    status: 'action',
    statusLabel: 'Needs action',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#0f6eeb]',
  },
  {
    title: 'Missing Log',
    subtitle: 'System Generated',
    date: 'Jan 02, 2025',
    status: 'resolution',
    statusLabel: 'Needs resolution',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#cc5c02]',
  },
  {
    title: 'Leave Request',
    subtitle: 'Jane Doe',
    date: 'Dec 28, 2024',
    status: 'action',
    statusLabel: 'Needs action',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#0f6eeb]',
  },
  {
    title: 'Overtime Request',
    subtitle: 'Jane Doe',
    date: 'Dec 20, 2024',
    status: 'action',
    statusLabel: 'Needs action',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#0f6eeb]',
  },
]
</script>
```

**Step 2: Verify**

Open `http://localhost:5173/dashboard` — tasks widget shows 5 task rows with status lozenges.

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/views/dashboard/TasksWidget.vue
git commit -m "feat: implement TasksWidget with 5 mock task rows"
```

---

### Task 7: Build PayslipsWidget component

**Files:**
- Modify: `Hackathon.Frontend/src/views/dashboard/PayslipsWidget.vue`

**Step 1: Implement PayslipsWidget**

Replace `Hackathon.Frontend/src/views/dashboard/PayslipsWidget.vue` with:

```vue
<template>
  <div class="bg-white rounded-2xl overflow-hidden">
    <!-- Header -->
    <div class="flex items-center justify-between px-4 py-3 border-b border-[#eff1f1]">
      <span class="text-base font-medium text-[#00291b]">Payslips</span>
    </div>

    <!-- Latest Transactions -->
    <div class="px-4 pt-3">
      <span class="text-sm font-medium text-[#738482]">Latest Transactions</span>
    </div>

    <!-- Transaction rows -->
    <div class="divide-y divide-[#eff1f1] px-4">
      <div
        v-for="tx in transactions"
        :key="tx.period"
        class="flex items-center justify-between py-3"
      >
        <div class="flex flex-col">
          <div class="flex items-center gap-2">
            <span class="text-sm font-medium text-[#262b2b]">{{ tx.type }}</span>
          </div>
          <span class="text-sm text-[#5d6c6b]">{{ tx.period }}</span>
          <span class="text-sm text-[#5d6c6b]">{{ tx.note }}</span>
        </div>
        <div class="flex items-center gap-1">
          <span class="text-sm font-medium text-[#262b2b]">{{ tx.amount }}</span>
          <span class="text-sm text-[#738482]">{{ tx.currency }}</span>
        </div>
      </div>
    </div>

    <!-- Footer actions -->
    <div class="flex flex-col gap-2 px-4 pb-4 pt-2">
      <button class="flex items-center gap-1 text-sm font-medium text-[#262b2b]">
        <Icon icon="ph:download-simple" class="text-base" />
        Download BIR 2316
      </button>
      <button class="flex items-center gap-1 text-sm font-medium text-[#262b2b]">
        <Icon icon="ph:eye" class="text-base" />
        View Payroll Summary
      </button>
      <button class="bg-[#158039] text-white rounded-lg px-3 py-2 text-sm font-medium w-full mt-1">
        Primary
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'

const transactions = [
  {
    type: 'Payroll',
    period: 'Dec 16 - 31, 2025',
    note: 'Late & Absence',
    amount: '******',
    currency: 'PHP',
  },
  {
    type: 'Payroll',
    period: 'Dec 01 - 15, 2025',
    note: 'Late & Absence',
    amount: '******',
    currency: 'PHP',
  },
  {
    type: '13th Month Pay',
    period: '2024',
    note: 'Late & Absence',
    amount: '******',
    currency: 'PHP',
  },
]
</script>
```

**Step 2: Verify**

Open `http://localhost:5173/dashboard` — payslips widget shows 3 transactions with masked amounts and action buttons.

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/views/dashboard/PayslipsWidget.vue
git commit -m "feat: implement PayslipsWidget with mock transactions"
```

---

### Task 8: Build LeaveWidget component

**Files:**
- Modify: `Hackathon.Frontend/src/views/dashboard/LeaveWidget.vue`

**Step 1: Implement LeaveWidget**

Replace `Hackathon.Frontend/src/views/dashboard/LeaveWidget.vue` with:

```vue
<template>
  <div class="bg-white rounded-2xl overflow-hidden">
    <!-- Header with tabs -->
    <div class="px-4 pt-3 pb-0">
      <div class="flex items-center gap-4 mb-3">
        <span class="text-sm font-medium text-[#262b2b]">Leave Credits</span>
        <span class="bg-[#da2f38] text-white text-xs font-medium rounded-full w-5 h-5 flex items-center justify-center">8</span>
        <span class="text-sm font-medium text-[#262b2b] ml-4">As employee</span>
        <span class="bg-[#da2f38] text-white text-xs font-medium rounded-full w-5 h-5 flex items-center justify-center">8</span>
        <span class="text-sm font-medium text-[#262b2b] ml-4">active requests</span>
        <span class="bg-[#da2f38] text-white text-xs font-medium rounded-full w-5 h-5 flex items-center justify-center">8</span>
      </div>

      <!-- Tabs -->
      <div class="flex gap-1 border-b border-[#eff1f1]">
        <button
          v-for="tab in tabs"
          :key="tab"
          class="px-3 py-2 text-sm transition-colors relative"
          :class="activeTab === tab
            ? 'text-[#158039] font-medium'
            : 'text-[#262b2b]'"
          @click="activeTab = tab"
        >
          {{ tab }}
          <span
            v-if="activeTab === tab"
            class="absolute bottom-0 left-0 right-0 h-0.5 bg-[#158039]"
          />
        </button>
      </div>
    </div>

    <!-- AVAILABLE CREDITS header -->
    <div class="px-4 pt-3 pb-2">
      <span class="text-sm font-medium text-[#738482] tracking-wide">AVAILABLE CREDITS</span>
    </div>

    <!-- Leave type rows -->
    <div class="divide-y divide-[#eff1f1] max-h-[260px] overflow-y-auto">
      <div
        v-for="leave in leaveTypes"
        :key="leave.name"
        class="flex items-center justify-between px-4 py-2"
      >
        <span class="text-sm text-[#5d6c6b]">{{ leave.name }}</span>
        <div class="flex items-center gap-1">
          <span class="text-xl font-medium text-[#158039]">{{ leave.used }}</span>
          <span class="text-sm text-[#738482]">/</span>
          <span class="text-sm text-[#738482]">{{ leave.total }}</span>
          <span class="text-xs text-[#738482] ml-0.5">days</span>
        </div>
      </div>
    </div>

    <!-- Footer actions -->
    <div class="flex items-center justify-between px-4 py-3 border-t border-[#eff1f1]">
      <button class="flex items-center gap-1 text-sm font-medium text-[#262b2b]">
        <Icon icon="ph:eye" class="text-base" />
        View All Requests
      </button>
      <button class="flex items-center gap-1 bg-[#158039] text-white rounded-lg px-3 py-2 text-sm font-medium">
        <Icon icon="ph:plus" class="text-base" />
        Apply Request
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Icon } from '@iconify/vue'

const tabs = ['This Pay Period', 'This Month', 'Year-To-Date']
const activeTab = ref('This Pay Period')

const leaveTypes = [
  { name: 'Vacation', used: 20, total: 40 },
  { name: 'Sick', used: 20, total: 40 },
  { name: 'Emergency', used: 20, total: 40 },
  { name: 'Magna Carta', used: 20, total: 40 },
  { name: 'Paternity', used: 20, total: 40 },
  { name: 'Service Incentive', used: 20, total: 40 },
  { name: 'Bereavement', used: 20, total: 40 },
  { name: 'Comprehensive Family Care and Support Leave', used: 20, total: 40 },
]
</script>
```

**Step 2: Verify**

Open `http://localhost:5173/dashboard` — leave widget shows tabs, credit counters, and 8 leave type rows with balances.

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/views/dashboard/LeaveWidget.vue
git commit -m "feat: implement LeaveWidget with tabs and mock leave balances"
```

---

### Task 9: Final polish and visual verification

**Files:**
- Possibly tweak: any of the dashboard components for spacing/alignment fixes

**Step 1: Run the app and compare against Figma**

Run: `npm run dev`

Open `http://localhost:5173/dashboard` and visually compare against the Figma design. Check:
- Sidebar width and icon alignment
- Announcement bar spacing
- Profile card decorative border and button alignment
- Two-column grid proportions
- Task row alignment and status lozenge colors
- Leave widget tab underline and credit display
- Payslips transaction layout

**Step 2: Run type-check**

Run: `cd Hackathon.Frontend && npx vue-tsc --noEmit`

Fix any TypeScript errors.

**Step 3: Verify existing app is untouched**

Open `http://localhost:5173/` — existing PayFeedScreen flow should work identically.

**Step 4: Commit any fixes**

```bash
git add -A Hackathon.Frontend/src/views/dashboard/
git commit -m "fix: polish dashboard layout and fix visual discrepancies"
```

---

## Summary

| Task | Description | Files |
|------|-------------|-------|
| 1 | Install vue-router, create router.ts | router.ts, package.json |
| 2 | Extract RunwayApp.vue, wire up router | RunwayApp.vue, App.vue, main.ts |
| 3 | Dashboard shell + sidebar | DashboardPage.vue, DashboardSidebar.vue, SidebarIcon.vue, stubs |
| 4 | AnnouncementBar | AnnouncementBar.vue |
| 5 | ProfileCard | ProfileCard.vue |
| 6 | TasksWidget | TasksWidget.vue |
| 7 | PayslipsWidget | PayslipsWidget.vue |
| 8 | LeaveWidget | LeaveWidget.vue |
| 9 | Polish + verify | All dashboard files |
