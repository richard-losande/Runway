<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8">
    <div class="w-full max-w-lg">
      <!-- Survival Days Hero -->
      <div class="text-center mb-8">
        <p class="text-sm text-gray-500 mb-2">Your financial runway</p>
        <p class="text-7xl font-bold text-gray-900">{{ displayDays }}</p>
        <p class="text-lg text-gray-500 mt-2">days</p>
        <p class="text-sm text-gray-500 mt-1">{{ sp2.baseline.humanLabel }}</p>
      </div>

      <!-- Stability Zone Badge -->
      <div class="flex justify-center mb-6">
        <span
          class="px-4 py-1 rounded-full text-sm font-semibold"
          :style="{ backgroundColor: zoneColor + '20', color: zoneColor }"
        >
          {{ zoneLabel }}
        </span>
      </div>

      <!-- Zone Legend -->
      <div class="flex justify-center gap-3 mb-8 flex-wrap text-xs text-gray-500">
        <span><span class="inline-block w-2 h-2 rounded-full mr-1 align-middle" style="background: #F43F5E;"></span>Critical &lt;30</span>
        <span><span class="inline-block w-2 h-2 rounded-full mr-1 align-middle" style="background: #F97316;"></span>Fragile 30-59</span>
        <span><span class="inline-block w-2 h-2 rounded-full mr-1 align-middle" style="background: #EAB308;"></span>Stable 60-119</span>
        <span><span class="inline-block w-2 h-2 rounded-full mr-1 align-middle" style="background: #10B981;"></span>Strong 120+</span>
      </div>

      <!-- Scenario Toggle Cards -->
      <div class="space-y-3 mb-6">
        <div
          v-for="scenario in sp2.scenarios"
          :key="scenario.id"
          class="bg-white rounded-lg shadow p-4 flex items-center justify-between cursor-pointer hover:shadow-md transition-shadow"
          @click="onToggle(scenario.id)"
        >
          <div>
            <p class="text-sm font-semibold text-gray-900">
              {{ scenario.label }}
              <span
                v-if="scenario.isPriority"
                class="ml-2 px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-700"
              >
                Recommended
              </span>
            </p>
            <p
              class="text-sm mt-1 font-medium"
              :class="scenario.deltaDays >= 0 ? 'text-green-600' : 'text-red-600'"
            >
              {{ scenario.deltaDays >= 0 ? '+' : '' }}{{ scenario.deltaDays }} days
            </p>
          </div>
          <div
            class="w-10 h-6 rounded-full relative cursor-pointer transition-colors duration-200"
            :class="store.activeScenarios.includes(scenario.id) ? 'bg-green-500' : 'bg-gray-300'"
          >
            <div
              class="w-4 h-4 bg-white rounded-full absolute top-1 transition-all duration-200"
              :class="store.activeScenarios.includes(scenario.id) ? 'left-5' : 'left-1'"
            ></div>
          </div>
        </div>
      </div>

      <!-- Max Warning -->
      <p
        v-if="store.activeScenarios.length >= 3"
        class="text-sm text-gray-500 text-center mb-4"
      >
        You've hit the max — deselect one to try another
      </p>

      <!-- Stacked Result -->
      <div v-if="store.activeScenarios.length > 0" class="text-center mb-6 p-4 bg-gray-50 rounded-lg">
        <p class="text-sm text-gray-500">With selected scenarios</p>
        <p class="text-3xl font-bold text-gray-900">{{ sp2.stackedResult.survivalDays }} days</p>
        <p
          class="text-sm font-medium"
          :class="sp2.stackedResult.deltaDays >= 0 ? 'text-green-600' : 'text-red-600'"
        >
          {{ sp2.stackedResult.deltaDays >= 0 ? '+' : '' }}{{ sp2.stackedResult.deltaDays }} from baseline
        </p>
      </div>

      <!-- CTA -->
      <button
        @click="store.revealMyProfile()"
        :disabled="store.isLoading"
        class="w-full py-3 px-4 rounded-md bg-green-600 hover:bg-green-700 text-white font-semibold text-base disabled:opacity-50"
      >
        {{ store.isLoading ? 'Analyzing...' : 'Reveal My Profile →' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()
const sp2 = computed(() => store.sp2Result!)

const displayDays = computed(() =>
  store.activeScenarios.length > 0
    ? sp2.value.stackedResult.survivalDays
    : sp2.value.baseline.survivalDays
)

const currentZone = computed(() => {
  const days = displayDays.value
  if (days < 30) return 'critical'
  if (days < 60) return 'fragile'
  if (days < 120) return 'stable'
  return 'strong'
})

const zoneColor = computed(() => {
  switch (currentZone.value) {
    case 'critical': return '#F43F5E'
    case 'fragile': return '#F97316'
    case 'stable': return '#EAB308'
    case 'strong': return '#10B981'
    default: return '#6B7280'
  }
})

const zoneLabel = computed(() => {
  switch (currentZone.value) {
    case 'critical': return 'One paycheck away'
    case 'fragile': return 'Worth paying attention to'
    case 'stable': return 'Doing okay \u2014 room to improve'
    case 'strong': return "You're in good shape"
    default: return ''
  }
})

function onToggle(scenarioId: string) {
  store.toggleScenario(scenarioId)
}
</script>
