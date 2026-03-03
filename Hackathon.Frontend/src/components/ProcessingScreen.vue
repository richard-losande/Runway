<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8">
    <div class="w-full max-w-md text-center">
      <h2 class="text-2xl font-bold text-gray-900 mb-8">Analyzing your transactions...</h2>

      <div class="text-left space-y-3">
        <div
          v-for="(line, index) in visibleLines"
          :key="index"
          class="flex items-center gap-2 text-sm"
        >
          <span class="text-green-600 w-5 text-center">{{ line.icon }}</span>
          <span :class="line.bold ? 'font-semibold text-gray-900' : 'text-gray-600'">
            {{ line.text }}
          </span>
        </div>
      </div>

      <div v-if="visibleLines.length < feedLines.length" class="mt-8">
        <div class="w-full bg-gray-200 rounded-full h-2">
          <div
            class="bg-green-500 h-2 rounded-full transition-all duration-300"
            :style="{ width: progress + '%' }"
          ></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'

const feedLines = [
  { icon: '\u2713', text: '847 transactions found', bold: true },
  { icon: '\u2713', text: 'Removing duplicates', bold: false },
  { icon: '\u27f3', text: 'Categorizing your spend...', bold: true },
  { icon: ' ', text: 'Grab Food \u2192 Dining', bold: false },
  { icon: ' ', text: 'Shopee \u2192 Shopping', bold: false },
  { icon: ' ', text: 'Meralco \u2192 Utilities', bold: false },
  { icon: ' ', text: 'Netflix \u2192 Subscriptions', bold: false },
  { icon: ' ', text: 'Mercury Drug \u2192 Healthcare', bold: false },
  { icon: '\u2713', text: 'Building burn profile...', bold: true },
  { icon: '\u2713', text: 'Detecting danger signals...', bold: true },
]

const visibleCount = ref(0)
const visibleLines = computed(() => feedLines.slice(0, visibleCount.value))
const progress = computed(() => Math.round((visibleCount.value / feedLines.length) * 100))

let timer: ReturnType<typeof setInterval>

onMounted(() => {
  timer = setInterval(() => {
    if (visibleCount.value < feedLines.length) {
      visibleCount.value++
    }
  }, 400)
})

onUnmounted(() => {
  clearInterval(timer)
})
</script>
