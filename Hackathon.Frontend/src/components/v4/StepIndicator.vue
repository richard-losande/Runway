<template>
  <div class="flex items-center">
    <template v-for="step in 8" :key="step">
      <!-- Circle -->
      <div
        class="w-7 h-7 rounded-full flex items-center justify-center text-xs font-semibold flex-shrink-0"
        :class="{
          'bg-green-500 text-white': step < current,
          'border-2 border-green-500 text-green-600': step === current,
          'border-2 border-gray-300 text-gray-400': step > current,
        }"
      >
        <svg v-if="step < current" class="w-3.5 h-3.5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" />
        </svg>
        <span v-else>{{ step }}</span>
      </div>
      <!-- Connecting line -->
      <div
        v-if="step < 8"
        class="flex-1 h-0.5 mx-0.5"
        :class="step < current ? 'bg-green-500' : 'bg-gray-200'"
      />
    </template>
  </div>
</template>

<script setup lang="ts">
defineProps<{ current: number }>()
</script>
