<template>
  <div class="max-w-md mx-auto p-4 space-y-5">
    <!-- Archetype Card (Dark Green Gradient) -->
    <div class="rounded-2xl overflow-hidden bg-gradient-to-br from-green-800 to-emerald-900 text-white p-6">
      <p class="text-xs font-semibold uppercase tracking-widest text-green-300 mb-2">Your Pattern</p>
      <template v-if="!store.isDiagnosing && store.diagnosis">
        <h2 class="text-2xl font-bold mb-2">{{ store.diagnosis.archetypeName }}</h2>
        <p class="text-sm text-green-100 leading-relaxed">
          {{ store.insightProfile?.archetype.signal }}
        </p>
      </template>
      <!-- Shimmer loading state -->
      <template v-else>
        <div class="space-y-3 animate-pulse">
          <div class="h-7 bg-green-700/50 rounded-lg w-3/4" />
          <div class="h-4 bg-green-700/50 rounded w-full" />
          <div class="h-4 bg-green-700/50 rounded w-5/6" />
        </div>
      </template>
    </div>

    <!-- Diagnosis Body Card -->
    <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
      <template v-if="!store.isDiagnosing && store.diagnosis">
        <!-- What's happening -->
        <div class="p-5 border-b border-gray-100">
          <h3 class="text-xs font-semibold uppercase tracking-wide text-gray-400 mb-2">What's happening</h3>
          <p class="text-sm text-gray-700 leading-relaxed">{{ store.diagnosis.whatIsHappening }}</p>
        </div>

        <!-- What to do about it -->
        <div class="p-5 border-b border-gray-100">
          <h3 class="text-xs font-semibold uppercase tracking-wide text-gray-400 mb-2">What to do about it</h3>
          <p class="text-sm text-gray-700 leading-relaxed">{{ store.diagnosis.whatToDoAboutIt }}</p>

          <!-- Before/After runway -->
          <div class="mt-3 flex items-center gap-3">
            <div class="flex items-center gap-2 bg-gray-50 rounded-lg px-3 py-2">
              <span class="text-xs text-gray-500">Before</span>
              <span class="text-base font-bold text-gray-700">{{ store.baselineDays }}d</span>
            </div>
            <svg class="w-4 h-4 text-green-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
            <div class="flex items-center gap-2 bg-green-50 rounded-lg px-3 py-2">
              <span class="text-xs text-green-600">After</span>
              <span class="text-base font-bold text-green-700">{{ afterDays }}d</span>
            </div>
          </div>
        </div>

        <!-- The honest take -->
        <div class="p-5">
          <h3 class="text-xs font-semibold uppercase tracking-wide text-gray-400 mb-2">The honest take</h3>
          <p class="text-sm text-gray-600 leading-relaxed italic">{{ store.diagnosis.honestTake }}</p>
        </div>
      </template>

      <!-- Shimmer loading state -->
      <template v-else>
        <div class="p-5 space-y-6 animate-pulse">
          <div class="space-y-2">
            <div class="h-3 bg-gray-200 rounded w-1/3" />
            <div class="h-4 bg-gray-100 rounded w-full" />
            <div class="h-4 bg-gray-100 rounded w-5/6" />
          </div>
          <div class="space-y-2">
            <div class="h-3 bg-gray-200 rounded w-2/5" />
            <div class="h-4 bg-gray-100 rounded w-full" />
            <div class="h-4 bg-gray-100 rounded w-4/5" />
          </div>
          <div class="space-y-2">
            <div class="h-3 bg-gray-200 rounded w-1/4" />
            <div class="h-4 bg-gray-100 rounded w-full" />
            <div class="h-4 bg-gray-100 rounded w-3/4" />
          </div>
        </div>
      </template>
    </div>

    <!-- Attribution -->
    <p class="text-xs text-gray-400 text-center italic">
      Generated from 247 transactions &middot; Sprout AI
    </p>

    <!-- CTA Button -->
    <button
      class="w-full py-3.5 px-6 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-xl transition-colors duration-150 text-base flex items-center justify-center gap-2"
      :disabled="store.isDiagnosing"
      :class="{ 'opacity-50 cursor-not-allowed': store.isDiagnosing }"
      @click="store.goToScreen(8)"
    >
      What's My Next Move?
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
      </svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()

const afterDays = computed(() => {
  const fw = store.fastestWin
  return fw ? store.baselineDays + fw.delta : store.baselineDays
})
</script>
