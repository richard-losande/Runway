<template>
  <div class="flex flex-col min-h-screen max-w-md mx-auto">
  <div class="flex-1 overflow-y-auto p-4 pb-24 space-y-5">
    <!-- Large Runway Number -->
    <div class="text-center pt-4">
      <p
        class="font-bold leading-none"
        :style="{ fontSize: '88px', color: currentZoneConfig.colour }"
      >
        {{ store.displayDays }}
      </p>
      <p class="text-lg text-gray-500 font-medium -mt-1">days</p>
    </div>

    <!-- Zone Gradient Bar -->
    <div class="relative px-2">
      <div
        class="h-3 rounded-full"
        style="background: linear-gradient(to right, #E53E3E 0%, #DD6B20 25%, #3182CE 50%, #38A169 75%, #38A169 100%);"
      >
        <!-- Marker dot -->
        <div
          class="absolute top-1/2 -translate-y-1/2 w-5 h-5 bg-white rounded-full border-2 shadow-md transition-all duration-500"
          :style="{
            left: markerPosition + '%',
            borderColor: currentZoneConfig.colour
          }"
        />
      </div>
      <!-- Zone labels -->
      <div class="flex justify-between mt-1.5 text-[10px] text-gray-400 px-1">
        <span>Critical</span>
        <span>Fragile</span>
        <span>Stable</span>
        <span>Strong</span>
      </div>
    </div>

    <!-- Zone Callout Card -->
    <div
      class="rounded-xl border-2 px-4 py-3"
      :style="{ borderColor: currentZoneConfig.colour + '40', backgroundColor: currentZoneConfig.colour + '08' }"
    >
      <div class="flex items-center gap-2 mb-1">
        <span
          class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold text-white"
          :style="{ backgroundColor: currentZoneConfig.colour }"
        >
          {{ currentZoneConfig.label }}
        </span>
      </div>
      <p class="text-sm text-gray-600">{{ currentZoneConfig.description }}</p>
    </div>

    <!-- Fastest Win Banner -->
    <div v-if="store.fastestWin" class="bg-green-50 border border-green-200 rounded-xl px-4 py-3">
      <p class="text-xs font-semibold text-green-700 uppercase tracking-wide mb-1">Your fastest win</p>
      <div class="flex items-center justify-between">
        <span class="text-sm font-medium text-green-900">{{ store.fastestWin.label }}</span>
        <span class="text-sm font-bold text-green-700">
          +{{ store.fastestWin.delta }} days
        </span>
      </div>
    </div>

    <!-- Scenario Chips -->
    <div>
      <h3 class="text-sm font-semibold text-gray-700 mb-2">What-if scenarios</h3>
      <div class="flex flex-wrap gap-2">
        <button
          v-for="scenario in store.scenarios"
          :key="scenario.id"
          class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full text-sm font-medium border transition-all duration-150"
          :class="isActive(scenario.id)
            ? 'bg-green-100 border-green-300 text-green-800'
            : 'bg-white border-gray-200 text-gray-700 hover:border-green-300'"
          @click="store.toggleScenario(scenario.id)"
        >
          {{ scenario.label }}
          <span
            class="text-xs font-bold px-1.5 py-0.5 rounded-full"
            :class="scenario.delta >= 0 ? 'bg-green-200 text-green-800' : 'bg-red-200 text-red-800'"
          >
            {{ scenario.delta >= 0 ? '+' : '' }}{{ scenario.delta }}
          </span>
        </button>

        <!-- Applied custom scenario chips -->
        <button
          v-for="cs in store.customScenarios"
          :key="cs.id"
          class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full text-sm font-medium border transition-all duration-150"
          :class="isActive(cs.id)
            ? 'bg-green-100 border-green-300 text-green-800'
            : 'bg-white border-gray-200 text-gray-700 hover:border-green-300'"
          @click="store.toggleScenario(cs.id)"
        >
          {{ cs.label }}
          <span
            class="text-xs font-bold px-1.5 py-0.5 rounded-full"
            :class="cs.delta >= 0 ? 'bg-green-200 text-green-800' : 'bg-red-200 text-red-800'"
          >
            {{ cs.delta >= 0 ? '+' : '' }}{{ cs.delta }}
          </span>
        </button>

        <!-- Add custom scenario button -->
        <button
          v-if="!showCustomForm"
          class="inline-flex items-center gap-1 px-3 py-1.5 rounded-full text-sm font-medium border-2 border-dashed border-gray-300 text-gray-500 hover:border-green-400 hover:text-green-600 transition-colors"
          @click="showCustomForm = true"
        >
          + Custom scenario
        </button>
      </div>

      <!-- Custom scenario inline form -->
      <div v-if="showCustomForm" class="mt-3 bg-white rounded-xl border border-gray-200 p-4 space-y-3">
        <div class="space-y-1.5">
          <label class="text-xs font-medium text-gray-600">Label</label>
          <input
            v-model="customLabel"
            type="text"
            placeholder="e.g. Side hustle income"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none"
          />
        </div>
        <div class="space-y-1.5">
          <label class="text-xs font-medium text-gray-600">Monthly amount (&#8369;)</label>
          <input
            v-model.number="customAmount"
            type="number"
            placeholder="5000"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none"
          />
        </div>
        <div class="flex gap-2">
          <button
            class="flex-1 py-2 px-4 bg-green-600 hover:bg-green-700 text-white font-medium rounded-lg text-sm transition-colors"
            @click="applyCustom"
          >
            Apply
          </button>
          <button
            class="py-2 px-4 bg-gray-100 hover:bg-gray-200 text-gray-600 font-medium rounded-lg text-sm transition-colors"
            @click="showCustomForm = false"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>

    <!-- Stacked Result Panel -->
    <div
      v-if="store.activeScenarioIds.length > 0"
      class="bg-white rounded-2xl shadow-sm border border-gray-200 p-5"
    >
      <div class="text-center">
        <p class="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-1">
          With {{ store.activeScenarioIds.length }} scenario{{ store.activeScenarioIds.length > 1 ? 's' : '' }} applied
        </p>
        <p class="text-4xl font-bold" :style="{ color: stackedZoneConfig.colour }">
          {{ store.stackedDays }}
        </p>
        <p class="text-sm text-gray-500">days</p>
        <div class="flex items-center justify-center gap-2 mt-2">
          <span
            class="text-sm font-bold"
            :class="store.stackedDelta >= 0 ? 'text-green-600' : 'text-red-600'"
          >
            {{ store.stackedDelta >= 0 ? '+' : '' }}{{ store.stackedDelta }} days
          </span>
          <span
            class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-bold text-white"
            :style="{ backgroundColor: stackedZoneConfig.colour }"
          >
            {{ stackedZoneConfig.label }}
          </span>
        </div>
        <p v-if="store.stackedDate" class="text-xs text-gray-400 mt-1">
          Runway until {{ store.stackedDate }}
        </p>
      </div>
    </div>

    <!-- Reverse Mode -->
    <div class="space-y-2">
      <button
        v-if="!showReverseInput"
        class="text-sm font-medium text-green-600 hover:text-green-700 transition-colors"
        @click="showReverseInput = true"
      >
        Set a target &rarr;
      </button>
      <div v-else class="bg-white rounded-xl border border-gray-200 p-4 space-y-3">
        <label class="text-xs font-medium text-gray-600">Target runway (days)</label>
        <input
          v-model.number="reverseTargetInput"
          type="number"
          placeholder="120"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none"
        />
        <div class="flex gap-2">
          <button
            class="flex-1 py-2 px-4 bg-green-600 hover:bg-green-700 text-white font-medium rounded-lg text-sm transition-colors"
            @click="applyReverseTarget"
          >
            Calculate
          </button>
          <button
            class="py-2 px-4 bg-gray-100 hover:bg-gray-200 text-gray-600 font-medium rounded-lg text-sm transition-colors"
            @click="showReverseInput = false"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>

  </div>

  <!-- Fixed Footer -->
  <div class="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-100 px-4 py-4 flex gap-3 rounded-b-2xl">
    <spr-button variant="secondary" size="large" fullwidth @click="store.goToScreen(5)">
      Back
    </spr-button>
    <spr-button tone="success" size="large" fullwidth @click="revealProfile">
      Reveal My Profile
    </spr-button>
  </div>
</div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'
import { ZONE_CONFIG } from '../../lib/zones'

const store = useRunwayV4Store()

const showCustomForm = ref(false)
const customLabel = ref('')
const customAmount = ref(5000)
const showReverseInput = ref(false)
const reverseTargetInput = ref(120)

const currentZoneConfig = computed(() => ZONE_CONFIG[store.displayZone])
const stackedZoneConfig = computed(() => ZONE_CONFIG[store.stackedZone])

const markerPosition = computed(() => {
  // Map days to 0-100% across the gradient bar, capped at 160 days
  return Math.min(100, Math.max(0, (store.displayDays / 160) * 100))
})

function isActive(id: string): boolean {
  return store.activeScenarioIds.includes(id)
}

function revealProfile() {
  store.goToScreen(7)
  store.fetchDiagnosis()
}

function applyCustom() {
  const amount = Number(customAmount.value) || 0
  if (customLabel.value.trim() && amount !== 0) {
    store.setCustomScenario(customLabel.value.trim(), amount)
    showCustomForm.value = false
  }
}

function applyReverseTarget() {
  if (reverseTargetInput.value > 0) {
    store.setReverseTarget(reverseTargetInput.value)
    showReverseInput.value = false
  }
}
</script>
