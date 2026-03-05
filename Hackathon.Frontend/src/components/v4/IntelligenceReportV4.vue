<template>
  <div class="flex flex-col min-h-screen max-w-md mx-auto">
    <div class="flex-1 overflow-y-auto p-4 pb-24 space-y-5">
    <!-- Header -->
    <div>
      <h1 class="text-2xl font-bold text-gray-900">Here's where your money goes</h1>
      <p class="text-sm text-gray-500 mt-1">
        Based on 4 months of transaction data, averaged per month.
      </p>
    </div>

    <!-- Burn Breakdown Card -->
    <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
      <div class="px-5 pt-4 pb-2">
        <h2 class="text-base font-semibold text-gray-900">Monthly Breakdown</h2>
      </div>
      <div class="px-5 pb-4 space-y-3">
        <div
          v-for="key in categoryKeys"
          :key="key"
          class="space-y-1"
        >
          <div class="flex justify-between items-center">
            <span class="text-sm" :class="key === 'Income' ? 'text-green-700 font-medium' : 'text-gray-600'">
              {{ getCategoryLabel(key) }}
            </span>
            <span class="text-sm font-semibold" :class="key === 'Income' ? 'text-green-700' : 'text-gray-900'">
              {{ key === 'Income' ? '+' : '' }}&#8369;{{ getCategoryAmount(key).toLocaleString() }}
            </span>
          </div>
          <div class="h-2 bg-gray-100 rounded-full overflow-hidden">
            <div
              class="h-full rounded-full transition-all duration-500"
              :class="key === 'Income' ? 'bg-green-400' : 'bg-green-500'"
              :style="{ width: getBarWidth(key) + '%' }"
            />
          </div>
          <div v-if="key === 'Income'" class="border-b border-gray-100 pb-2 mb-1" />
        </div>

        <!-- Total burn row -->
        <div class="flex justify-between items-center pt-3 border-t border-gray-200">
          <span class="text-base font-semibold text-gray-900">Total Monthly Burn</span>
          <span class="text-lg font-bold text-gray-900">
            &#8369;{{ totalBurn.toLocaleString() }}
          </span>
        </div>
      </div>
    </div>

    <!-- Danger Signal Cards -->
    <div v-if="store.dangerSignals.length > 0" class="space-y-3">
      <h2 class="text-base font-semibold text-gray-900">Signals to watch</h2>
      <div
        v-for="(signal, i) in store.dangerSignals.slice(0, 3)"
        :key="i"
        class="bg-white rounded-xl border overflow-hidden"
        :class="signal.severity === 'danger' ? 'border-red-200' : 'border-yellow-200'"
      >
        <div class="p-4">
          <div class="flex items-start gap-3">
            <div
              class="w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0"
              :class="signal.severity === 'danger' ? 'bg-red-100' : 'bg-yellow-100'"
            >
              <svg
                class="w-4 h-4"
                :class="signal.severity === 'danger' ? 'text-red-600' : 'text-yellow-600'"
                fill="none" stroke="currentColor" viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.268 16.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </div>
            <div class="flex-1">
              <div class="flex items-center gap-2 mb-0.5">
                <span
                  class="text-xs font-semibold uppercase"
                  :class="signal.severity === 'danger' ? 'text-red-700' : 'text-yellow-700'"
                >
                  {{ signal.severity }}
                </span>
              </div>
              <p class="text-sm font-semibold text-gray-900">{{ signal.title }}</p>
              <p class="text-sm text-gray-500 mt-0.5">{{ signal.detail }}</p>
              <span
                class="inline-block mt-2 text-xs font-medium px-2.5 py-1 rounded-full"
                :class="signal.severity === 'danger' ? 'bg-red-50 text-red-700' : 'bg-yellow-50 text-yellow-700'"
              >
                {{ signal.metric }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- "Does this look right?" collapsible -->
    <div class="bg-white rounded-xl border border-gray-200 overflow-hidden">
      <button
        class="w-full flex items-center justify-between p-4 text-left"
        @click="correctionsOpen = !correctionsOpen"
      >
        <span class="text-sm font-medium text-gray-700">Does this look right?</span>
        <svg
          class="w-5 h-5 text-gray-400 transition-transform duration-200"
          :class="{ 'rotate-180': correctionsOpen }"
          fill="none" stroke="currentColor" viewBox="0 0 24 24"
        >
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
        </svg>
      </button>
      <div v-if="correctionsOpen" class="px-4 pb-4 border-t border-gray-100">
        <p class="text-sm text-gray-500 mt-3">
          If some categories look off, you'll be able to adjust them in a future update.
          For now, these numbers are based on your actual transaction data.
        </p>
      </div>
    </div>

    </div>

    <!-- Fixed Footer -->
    <div class="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-100 px-4 py-4 flex gap-3 rounded-b-2xl">
      <spr-button variant="secondary" size="large" fullwidth @click="store.goToScreen(3)">
        Back
      </spr-button>
      <spr-button tone="success" size="large" fullwidth @click="store.goToScreen(6)">
        Get My Breakdown
      </spr-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'
import { CATEGORY_LABELS } from '../../lib/zones'
import type { CategoryKey } from '../../api/runway-v4-types'

const store = useRunwayV4Store()
const correctionsOpen = ref(false)

const categoryKeys = computed<CategoryKey[]>(() => {
  if (!store.categories) return []
  // Filter out zero-amount categories, put Income first, then sort expenses by amount descending
  const keys = (Object.keys(store.categories) as CategoryKey[])
    .filter(k => (store.categories![k]?.monthlyAverage ?? 0) > 0)
  const income = keys.filter(k => k === 'Income')
  const expenses = keys
    .filter(k => k !== 'Income')
    .sort((a, b) => (store.categories![b]?.monthlyAverage ?? 0) - (store.categories![a]?.monthlyAverage ?? 0))
  return [...income, ...expenses]
})

const totalBurn = computed(() => {
  if (!store.categories) return 0
  // Exclude Income from total burn
  return Object.entries(store.categories)
    .filter(([k]) => k !== 'Income')
    .reduce((sum, [, cat]) => sum + cat.monthlyAverage, 0)
})

function getCategoryLabel(key: CategoryKey): string {
  return CATEGORY_LABELS[key] ?? key
}

function getCategoryAmount(key: CategoryKey): number {
  return store.categories?.[key]?.monthlyAverage ?? 0
}

function getBarWidth(key: CategoryKey): number {
  if (totalBurn.value === 0) return 0
  return Math.min(95, (getCategoryAmount(key) / totalBurn.value) * 100)
}
</script>
