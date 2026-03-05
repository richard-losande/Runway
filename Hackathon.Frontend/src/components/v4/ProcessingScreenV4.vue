<template>
  <div class="flex flex-col items-center min-h-[80vh] px-4 pt-6" style="background: linear-gradient(180deg, #DED6FE 0%, #FFF 103.34%);">
    <!-- Sprout loader GIF -->
    <div class="mb-6">
      <img src="/sprout-loader.gif" alt="Analyzing..." class="w-20 h-20 object-contain" />
    </div>

    <!-- Title -->
    <h2 class="text-xl font-bold text-gray-900 mb-6 text-center">
      Analyzing your transactions...
    </h2>

    <!-- Category rows -->
    <div class="w-full">
      <div
        v-for="(cat, i) in categoryRows"
        :key="cat.key"
        class="flex items-center justify-between px-2 py-4 border-b border-gray-100 transition-all duration-500 ease-out"
        :class="i < visibleCount ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-3'"
      >
        <div class="flex items-center gap-3">
          <Icon :icon="cat.icon" class="text-gray-400 flex-shrink-0" style="width:22px;height:22px;" />
          <span class="text-sm font-medium text-gray-700">{{ cat.label }}</span>
        </div>
        <span class="text-sm font-bold text-gray-900">&#8369;{{ cat.amount.toLocaleString() }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { Icon } from '@iconify/vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()

const categoryRows = [
  { key: 'Housing',       icon: 'ph:house',             label: 'Housing & Utilities',    amount: 4200 },
  { key: 'FoodDining',    icon: 'ph:fork-knife',         label: 'Food & Dining',          amount: 3100 },
  { key: 'Transport',     icon: 'ph:car',                label: 'Transportation',         amount: 1870 },
  { key: 'Subscriptions', icon: 'ph:device-mobile',      label: 'Subscriptions & Bills',  amount: 890  },
  { key: 'Shopping',      icon: 'ph:shopping-bag',       label: 'Shopping',               amount: 1800 },
  { key: 'Entertainment', icon: 'ph:film-strip',         label: 'Entertainment',          amount: 950  },
  { key: 'Misc',          icon: 'ph:dots-three-circle',  label: 'Miscellaneous',          amount: 1390 },
]

const REVEAL_INTERVAL = 420   // ms between each card fade-in
const TOTAL_DURATION  = categoryRows.length * REVEAL_INTERVAL + 400

const elapsed = ref(0)
let timer: ReturnType<typeof setInterval> | null = null
let autoAdvanceTimer: ReturnType<typeof setTimeout> | null = null

const visibleCount = computed(() =>
  Math.min(categoryRows.length, Math.floor(elapsed.value / REVEAL_INTERVAL))
)

onMounted(() => {
  timer = setInterval(() => {
    elapsed.value += 50
    if (elapsed.value >= TOTAL_DURATION && timer) clearInterval(timer)
  }, 50)

  autoAdvanceTimer = setTimeout(checkAndAdvance, TOTAL_DURATION + 200)
})

onUnmounted(() => {
  if (timer) clearInterval(timer)
  if (autoAdvanceTimer) clearTimeout(autoAdvanceTimer)
})

function checkAndAdvance() {
  if (!store.isAnalyzing) {
    store.goToScreen(5)
  } else {
    const poll = setInterval(() => {
      if (!store.isAnalyzing) {
        clearInterval(poll)
        store.goToScreen(5)
      }
    }, 200)
  }
}
</script>
