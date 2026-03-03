<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8">
    <div class="w-full max-w-lg">
      <h2 class="text-2xl font-bold text-gray-900 mb-6">Here's what we found</h2>

      <!-- Burn Breakdown Bar -->
      <div class="bg-white rounded-lg shadow p-6 mb-6">
        <p class="text-sm font-semibold text-gray-700 mb-4">Monthly Burn Breakdown</p>
        <div class="flex h-8 rounded-md overflow-hidden mb-4">
          <div
            :style="{ width: fixedPct + '%' }"
            class="bg-blue-500 flex items-center justify-center text-xs font-bold text-white"
          >
            {{ fixedPct }}%
          </div>
          <div
            :style="{ width: variablePct + '%' }"
            class="bg-green-500 flex items-center justify-center text-xs font-bold text-white"
          >
            {{ variablePct }}%
          </div>
          <div
            :style="{ width: discretionaryPct + '%' }"
            class="bg-red-500 flex items-center justify-center text-xs font-bold text-white"
          >
            {{ discretionaryPct }}%
          </div>
        </div>
        <div class="flex justify-between text-sm text-gray-600">
          <span>
            <span class="inline-block w-3 h-3 bg-blue-500 rounded-sm mr-1 align-middle"></span>
            Fixed ₱{{ sp1.burnBreakdown.fixed.toLocaleString() }}
          </span>
          <span>
            <span class="inline-block w-3 h-3 bg-green-500 rounded-sm mr-1 align-middle"></span>
            Variable ₱{{ sp1.burnBreakdown.variable.toLocaleString() }}
          </span>
          <span>
            <span class="inline-block w-3 h-3 bg-red-500 rounded-sm mr-1 align-middle"></span>
            Lifestyle ₱{{ sp1.burnBreakdown.discretionary.toLocaleString() }}
          </span>
        </div>
        <p class="text-xl font-bold text-gray-900 mt-4 text-center">
          Total: ₱{{ sp1.monthlyBurn.toLocaleString() }}/month
        </p>
      </div>

      <!-- Danger Signals -->
      <div class="space-y-4 mb-6">
        <div
          v-for="signal in sp1.dangerSignals"
          :key="signal.category"
          class="bg-white rounded-lg shadow p-4"
          style="border-left: 4px solid #F97316;"
        >
          <p class="text-sm font-semibold text-gray-900">{{ signal.category }}</p>
          <p class="text-sm text-gray-600 mt-1">{{ signal.insight }}</p>
          <div class="flex gap-4 mt-2 text-sm">
            <span class="text-red-600 font-medium">↑ {{ (signal.monthlyGrowthRate * 100).toFixed(0) }}% growth</span>
            <span class="text-gray-500">₱{{ signal.monthlyAmount.toLocaleString() }}/mo</span>
          </div>
        </div>
      </div>

      <!-- CTA -->
      <button
        @click="store.goToScreen(4)"
        class="w-full py-3 px-4 rounded-md bg-green-600 hover:bg-green-700 text-white font-semibold text-base"
      >
        See My Survival Days →
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()
const sp1 = computed(() => store.sp1Result!)

const fixedPct = computed(() => Math.round((sp1.value.burnBreakdown.fixed / sp1.value.monthlyBurn) * 100))
const variablePct = computed(() => Math.round((sp1.value.burnBreakdown.variable / sp1.value.monthlyBurn) * 100))
const discretionaryPct = computed(() => 100 - fixedPct.value - variablePct.value)
</script>
