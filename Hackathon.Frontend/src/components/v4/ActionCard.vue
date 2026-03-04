<template>
  <div class="max-w-md mx-auto p-4 space-y-5">
    <!-- Completion State -->
    <template v-if="completed">
      <div class="text-center pt-12 pb-8 space-y-4">
        <div class="w-20 h-20 rounded-full bg-green-100 flex items-center justify-center mx-auto">
          <svg class="w-10 h-10 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <h2 class="text-xl font-bold text-gray-900">Your runway is saved</h2>
        <p class="text-sm text-gray-500">We'll keep tracking your spending and update your runway automatically.</p>
        <button
          class="text-sm font-medium text-green-600 hover:text-green-700 transition-colors mt-4"
          @click="store.restart()"
        >
          &larr; Back to start
        </button>
      </div>
    </template>

    <template v-else>
      <!-- Zone-Colored Problem Card -->
      <div
        class="rounded-2xl border-2 p-5"
        :style="{ borderColor: zoneConfig.colour + '40', backgroundColor: zoneConfig.colour + '08' }"
      >
        <div class="flex items-center gap-2 mb-3">
          <span
            class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold text-white"
            :style="{ backgroundColor: zoneConfig.colour }"
          >
            {{ zoneConfig.label }}
          </span>
          <span class="text-sm font-semibold text-gray-700">{{ store.displayDays }} days</span>
        </div>
        <p class="text-sm text-gray-700 leading-relaxed">{{ problemStatement }}</p>
      </div>

      <!-- Product Card -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
        <!-- ReadyWage (Critical) -->
        <template v-if="zoneConfig.product === 'ReadyWage'">
          <div class="p-5">
            <div class="flex items-center gap-3 mb-3">
              <div class="w-10 h-10 rounded-xl bg-red-100 flex items-center justify-center">
                <svg class="w-5 h-5 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div>
                <h3 class="text-base font-semibold text-gray-900">ReadyWage</h3>
                <p class="text-xs text-gray-500">Earned Wage Access</p>
              </div>
            </div>
            <p class="text-sm text-gray-600 mb-4">
              Access wages you've already earned before payday. No interest, no hidden fees.
            </p>
            <button
              class="w-full py-3 px-4 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-xl transition-colors text-sm"
              @click="completeCta"
            >
              Get My Earned Wages Now
            </button>
          </div>
        </template>

        <!-- ReadyCash (Fragile) -->
        <template v-else-if="zoneConfig.product === 'ReadyCash'">
          <div class="p-5">
            <div class="flex items-center gap-3 mb-3">
              <div class="w-10 h-10 rounded-xl bg-orange-100 flex items-center justify-center">
                <svg class="w-5 h-5 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              <div>
                <h3 class="text-base font-semibold text-gray-900">ReadyCash</h3>
                <p class="text-xs text-gray-500">Emergency Credit Line</p>
              </div>
            </div>
            <p class="text-sm text-gray-600 mb-4">
              A pre-approved credit line for when you need a bridge. Low rates, transparent terms.
            </p>
            <button
              class="w-full py-3 px-4 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-xl transition-colors text-sm"
              @click="completeCta"
            >
              Bridge the Gap with ReadyCash
            </button>
          </div>
        </template>

        <!-- ReadySave (Stable / Strong) -->
        <template v-else>
          <div class="p-5">
            <div class="flex items-center gap-3 mb-3">
              <div class="w-10 h-10 rounded-xl bg-blue-100 flex items-center justify-center">
                <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M3 6l3 1m0 0l-3 9a5.002 5.002 0 006.001 0M6 7l3 9M6 7l6-2m6 2l3-1m-3 1l-3 9a5.002 5.002 0 006.001 0M18 7l3 9m-3-9l-6-2m0-2v2m0 16V5m0 16H9m3 0h3" />
                </svg>
              </div>
              <div>
                <h3 class="text-base font-semibold text-gray-900">ReadySave</h3>
                <p class="text-xs text-gray-500">Automated Savings</p>
              </div>
            </div>
            <p class="text-sm text-gray-600 mb-4">
              Set aside a fixed amount each payday. Grow your runway automatically.
            </p>

            <!-- Amount chips -->
            <div class="flex flex-wrap gap-2 mb-4">
              <button
                v-for="amount in saveAmounts"
                :key="amount"
                class="px-4 py-2 rounded-full text-sm font-medium border transition-all"
                :class="selectedSaveAmount === amount
                  ? 'bg-green-100 border-green-300 text-green-800'
                  : 'bg-white border-gray-200 text-gray-700 hover:border-green-300'"
                @click="selectedSaveAmount = amount"
              >
                &#8369;{{ amount.toLocaleString() }}
              </button>
            </div>

            <button
              class="w-full py-3 px-4 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-xl transition-colors text-sm"
              @click="completeCta"
            >
              Start Saving &#8369;{{ (selectedSaveAmount ?? 2000).toLocaleString() }}/mo
            </button>
          </div>
        </template>
      </div>

      <!-- Maybe Later -->
      <div class="text-center">
        <button
          class="text-sm font-medium text-gray-400 hover:text-gray-600 transition-colors"
          @click="store.restart()"
        >
          Maybe later
        </button>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'
import { ZONE_CONFIG } from '../../lib/zones'

const store = useRunwayV4Store()
const completed = ref(false)
const selectedSaveAmount = ref<number | null>(2000)
const saveAmounts = [1000, 2000, 3000, 5000]

const zoneConfig = computed(() => ZONE_CONFIG[store.displayZone])

const burn = computed(() => store.state?.monthlyBurn ?? 0)
const savings = computed(() => store.state?.liquidCash ?? 0)

const problemStatement = computed(() => {
  const z = store.displayZone
  const d = store.displayDays
  const b = burn.value.toLocaleString()
  const s = savings.value.toLocaleString()

  switch (z) {
    case 'Critical':
      return `At \u20B1${b}/mo burn, your \u20B1${s} buffer runs out in ${d} days. You need breathing room today.`
    case 'Fragile':
      return `Your savings cover ${d} days. One unexpected expense could push you into the red.`
    case 'Stable':
      return `At \u20B1${b}/mo burn, your \u20B1${s} buffer runs out in ${d} days. The goal is to grow this before your spending pattern closes the gap.`
    case 'Strong':
      return `At ${d} days, your buffer is solid. The opportunity now is to make this money work harder while you have room.`
    default:
      return ''
  }
})

function completeCta() {
  completed.value = true
}
</script>
