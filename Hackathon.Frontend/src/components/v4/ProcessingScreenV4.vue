<template>
  <div class="max-w-md mx-auto p-4 flex flex-col items-center justify-center min-h-[80vh]">
    <!-- Progress Bar -->
    <div class="w-full mb-8">
      <div class="h-1.5 bg-gray-200 rounded-full overflow-hidden">
        <div
          class="h-full bg-green-500 rounded-full transition-all duration-100 ease-linear"
          :style="{ width: progressPct + '%' }"
        />
      </div>
      <p class="text-xs text-gray-400 text-center mt-2">{{ phaseLabel }}</p>
    </div>

    <!-- Phase 1: Raw transaction strings -->
    <div v-if="phase === 1" class="w-full space-y-1.5">
      <div
        v-for="(tx, i) in visibleTransactions"
        :key="i"
        class="font-mono text-xs text-gray-400 transition-opacity duration-180"
        :class="{ 'opacity-100': true }"
      >
        {{ tx }}
      </div>
    </div>

    <!-- Phase 2: Category rows -->
    <div
      v-else-if="phase === 2"
      class="w-full space-y-3"
    >
      <div
        v-for="(cat, i) in categoryRows"
        :key="cat.key"
        class="flex items-center gap-3 transition-opacity duration-300"
        :style="{ transitionDelay: (i * 80) + 'ms' }"
      >
        <span class="text-lg">{{ cat.icon }}</span>
        <span class="flex-1 text-sm font-medium text-gray-700">{{ cat.label }}</span>
        <span class="text-xs font-semibold text-gray-900 bg-gray-100 px-2.5 py-1 rounded-full">
          &#8369;{{ cat.amount.toLocaleString() }}
        </span>
      </div>
    </div>

    <!-- Phase 3: Analysis complete (or still waiting) -->
    <div v-else-if="phase === 3" class="text-center">
      <template v-if="store.isAnalyzing">
        <!-- Still waiting for API -->
        <div class="w-16 h-16 rounded-full bg-green-50 flex items-center justify-center mx-auto mb-4">
          <svg class="w-8 h-8 text-green-500 animate-spin" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
          </svg>
        </div>
        <p class="text-lg font-semibold text-gray-900">Still analyzing...</p>
        <p class="text-sm text-gray-500 mt-1">Processing your transactions, almost done.</p>
      </template>
      <template v-else>
        <!-- Done -->
        <div class="w-16 h-16 rounded-full bg-green-100 flex items-center justify-center mx-auto mb-4">
          <svg class="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <p class="text-lg font-semibold text-gray-900">Analysis complete</p>
        <p class="text-sm text-gray-500 mt-1">Your runway report is ready.</p>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()

const TOTAL_DURATION = 3200
const PHASE1_END = 1300
const PHASE2_END = 2400

const rawTransactions = [
  'GRAB*FOOD PHILIPPINES 09:42 \u20B1340.00',
  'GCASH SEND MONEY NENA G. \u20B15,000.00',
  'SM SUPERMARKET MOA 14:21 \u20B12,840.00',
  'MERALCO PAYMENT ECPay \u20B13,120.00',
  'LAZADA PAYMENTS PTE LTD \u20B11,299.00',
  'SHOPEE PAY SPAY-241103 \u20B1890.00',
]

const categoryRows = [
  { key: 'FoodDining', icon: '\uD83C\uDF5C', label: 'Food & Dining', amount: 8400 },
  { key: 'Transfers', icon: '\uD83D\uDCB8', label: 'Transfers & Family', amount: 5000 },
  { key: 'Groceries', icon: '\uD83D\uDED2', label: 'Groceries', amount: 4200 },
  { key: 'BillsUtilities', icon: '\u26A1', label: 'Bills & Utilities', amount: 3120 },
  { key: 'Shopping', icon: '\uD83D\uDECD\uFE0F', label: 'Shopping', amount: 2189 },
  { key: 'Transport', icon: '\uD83D\uDE95', label: 'Transport', amount: 1800 },
]

const elapsed = ref(0)
const visibleTxCount = ref(0)
let timer: ReturnType<typeof setInterval> | null = null
let autoAdvanceTimer: ReturnType<typeof setTimeout> | null = null

const phase = computed(() => {
  if (elapsed.value < PHASE1_END) return 1
  if (elapsed.value < PHASE2_END) return 2
  return 3
})

const phaseLabel = computed(() => {
  if (phase.value === 1) return 'Reading transactions...'
  if (phase.value === 2) return 'Categorizing spending...'
  return 'Done!'
})

const progressPct = computed(() => {
  return Math.min(100, (elapsed.value / TOTAL_DURATION) * 100)
})

const visibleTransactions = computed(() => {
  return rawTransactions.slice(0, visibleTxCount.value)
})

onMounted(() => {
  // Animate elapsed time
  const interval = 50
  timer = setInterval(() => {
    elapsed.value += interval

    // Phase 1: reveal transactions one by one
    if (elapsed.value < PHASE1_END) {
      const txInterval = PHASE1_END / rawTransactions.length
      visibleTxCount.value = Math.min(
        rawTransactions.length,
        Math.floor(elapsed.value / txInterval) + 1
      )
    }

    if (elapsed.value >= TOTAL_DURATION) {
      if (timer) clearInterval(timer)
    }
  }, interval)

  // Auto-advance after animation completes
  autoAdvanceTimer = setTimeout(() => {
    checkAndAdvance()
  }, TOTAL_DURATION + 200)
})

onUnmounted(() => {
  if (timer) clearInterval(timer)
  if (autoAdvanceTimer) clearTimeout(autoAdvanceTimer)
})

function checkAndAdvance() {
  // If analysis is done (isAnalyzing is false), go to screen 5
  if (!store.isAnalyzing) {
    store.goToScreen(5)
  } else {
    // Poll until analysis finishes
    const poll = setInterval(() => {
      if (!store.isAnalyzing) {
        clearInterval(poll)
        store.goToScreen(5)
      }
    }, 200)
  }
}
</script>
