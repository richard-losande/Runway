<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8">
    <div class="w-full max-w-lg">
      <!-- Archetype -->
      <div class="text-center mb-8">
        <p class="text-sm text-gray-500 mb-2">Your financial archetype</p>
        <h1 class="text-3xl font-bold text-gray-900">{{ sp3.archetype }}</h1>
      </div>

      <!-- Diagnosis -->
      <div class="bg-white rounded-lg shadow p-6 mb-6">
        <p class="text-base text-gray-800 leading-relaxed">
          {{ sp3.diagnosis }}
        </p>
      </div>

      <!-- Top Recommendation -->
      <div class="bg-gray-50 rounded-lg shadow p-6 mb-6" style="border-left: 4px solid #10B981;">
        <p class="text-sm font-semibold text-green-700 mb-2">Top Recommendation</p>
        <p class="text-base text-gray-800">
          {{ sp3.topRecommendation }}
        </p>
      </div>

      <!-- Closing Line -->
      <p class="text-sm text-gray-500 italic text-center mb-8">
        {{ sp3.closingLine }}
      </p>

      <!-- Restart -->
      <button
        @click="store.restart()"
        class="w-full py-3 px-4 rounded-md border border-gray-300 text-gray-700 font-semibold text-base hover:bg-gray-50"
      >
        ← Try Different Numbers
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()
const sp3 = computed(() => store.sp3Result!)
</script>
