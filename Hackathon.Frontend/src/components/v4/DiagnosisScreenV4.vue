<template>
  <div ref="containerRef" class="flex flex-col min-h-screen max-w-md mx-auto relative overflow-hidden">
    <div class="flex-1 overflow-y-auto p-4 pb-24 space-y-5" style="background: linear-gradient(180deg, #F0F9B3 3.59%, #FFF 44.25%);">

      <!-- Emoji + Archetype Header -->
      <div class="pt-4 pb-2">
        <div class="flex justify-center mb-5">
          <img :src="archetypeEmoji" alt="emoji" class="w-24 h-24" />
        </div>
        <p class="text-xs font-semibold text-gray-400 tracking-widest uppercase mb-1">You are a</p>

        <template v-if="!store.isDiagnosing">
          <h1 class="text-3xl font-black text-gray-900 mb-3">{{ archetypeName }}</h1>
          <p class="text-sm text-gray-600 leading-relaxed">{{ signal }}</p>
        </template>
        <template v-else>
          <div class="space-y-2 animate-pulse">
            <div class="h-8 bg-gray-200 rounded-lg w-2/3 mb-3" />
            <div class="h-4 bg-gray-100 rounded w-full" />
            <div class="h-4 bg-gray-100 rounded w-5/6" />
          </div>
        </template>
      </div>

      <!-- What's Happening Card -->
      <div class="bg-white rounded-2xl border border-gray-200 p-4">
        <template v-if="!store.isDiagnosing">
          <p class="text-sm text-gray-700 leading-relaxed">{{ whatIsHappening }}</p>
        </template>
        <template v-else>
          <div class="space-y-2 animate-pulse">
            <div class="h-4 bg-gray-100 rounded w-full" />
            <div class="h-4 bg-gray-100 rounded w-5/6" />
            <div class="h-4 bg-gray-100 rounded w-4/5" />
            <div class="h-4 bg-gray-100 rounded w-full" />
          </div>
        </template>
      </div>

      <!-- Top Recommendation Card -->
      <div class="rounded-2xl border-2 border-green-500 p-4">
        <p class="text-xs font-bold text-green-600 tracking-widest uppercase mb-2">Top Recommendation</p>
        <template v-if="!store.isDiagnosing">
          <p class="text-sm text-gray-700 leading-relaxed">{{ whatToDoAboutIt }}</p>
        </template>
        <template v-else>
          <div class="space-y-2 animate-pulse">
            <div class="h-4 bg-green-50 rounded w-full" />
            <div class="h-4 bg-green-50 rounded w-5/6" />
            <div class="h-4 bg-green-50 rounded w-3/4" />
          </div>
        </template>
      </div>

      <!-- Honest Take Quote -->
      <p v-if="!store.isDiagnosing" class="text-sm text-gray-400 italic leading-relaxed">
        {{ honestTake }}
      </p>

      <!-- Recommended For You — zone-to-product routing -->
      <div>
        <p class="text-xs font-semibold text-gray-400 tracking-widest uppercase mb-3">Recommended for you</p>
        <div class="bg-white rounded-2xl border border-gray-200 overflow-hidden">
          <div class="p-4 space-y-3">
            <p class="text-lg font-bold" :style="{ color: productConfig.color }">
              {{ productConfig.name }}
            </p>
            <p class="text-sm text-gray-600 leading-relaxed">
              {{ productConfig.description }}
            </p>
            <button
              class="w-full py-3 px-4 rounded-xl font-semibold text-sm text-white transition-colors"
              style="background: #1a4731;"
              @click="store.goToScreen(8)"
            >
              {{ productConfig.cta }}
            </button>
          </div>
        </div>
      </div>

    </div>

    <!-- Fixed Footer -->
    <div class="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-100 px-4 py-4 rounded-b-2xl">
      <spr-button variant="secondary" size="large" fullwidth @click="store.goToScreen(6)">
        Back
      </spr-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, watch } from 'vue'
import confetti from 'canvas-confetti'
import { useRunwayV4Store } from '../../stores/runway-v4'
import { ZONE_CONFIG } from '../../lib/zones'

const store = useRunwayV4Store()
const containerRef = ref<HTMLElement | null>(null)

const archetypeEmoji = computed(() => {
  const map: Record<string, string> = {
    Strong:   '/positive-emoji.svg',
    Stable:   '/stable.svg',
    Fragile:  '/fragile.svg',
    Critical: '/critical.svg',
  }
  return map[store.displayZone] ?? '/positive-emoji.svg'
})

const archetypeName = computed(() =>
  store.diagnosis?.archetypeName
  ?? store.insightProfile?.archetype?.name
  ?? 'Steady Spender'
)

const signal = computed(() =>
  store.insightProfile?.archetype?.signal
  || 'You have a consistent financial pattern with room to build resilience.'
)

const whatIsHappening = computed(() => {
  if (store.diagnosis?.whatIsHappening) return store.diagnosis.whatIsHappening
  const burn = store.state?.monthlyBurn?.toLocaleString('en-PH') ?? '—'
  const income = store.state?.takeHome?.toLocaleString('en-PH') ?? '—'
  const days = store.displayDays
  return `Your monthly burn of ₱${burn} against a ₱${income} income leaves a buffer every month. You have ${days} days of runway — and the habits to grow it.`
})

const whatToDoAboutIt = computed(() =>
  store.diagnosis?.whatToDoAboutIt
  ?? 'Automate a portion of your surplus into a locked savings account the day after payroll. This single move can add weeks to your runway without changing your lifestyle.'
)

const honestTake = computed(() =>
  store.diagnosis?.honestTake
  ?? "You're not just surviving — you're building. Most people never get this far."
)

const productConfig = computed(() => {
  const zone = store.displayZone
  const product = ZONE_CONFIG[zone]?.product ?? 'ReadySave'

  const configs = {
    ReadyWage: {
      name: 'ReadyWage',
      color: '#E53E3E',
      description: 'Get immediate access to your earned wages — the fastest path to cash when you need breathing room.',
      cta: 'Get My Earned Wages Now →',
    },
    ReadyCash: {
      name: 'ReadyCash',
      color: '#DD6B20',
      description: 'Bridge the gap with an interest-free emergency loan. Cover the unexpected without breaking your runway.',
      cta: 'Bridge the Gap →',
    },
    ReadySave: {
      name: 'ReadySave',
      color: '#38A169',
      description: 'Start building a buffer automatically. Automate your surplus and grow your runway every month.',
      cta: 'Start Saving →',
    },
  }

  return configs[product]
})

function launchConfetti() {
  if (!containerRef.value) return
  const rect = containerRef.value.getBoundingClientRect()
  const canvas = document.createElement('canvas')
  canvas.style.cssText = 'position:absolute;top:0;left:0;width:100%;height:100%;pointer-events:none;z-index:50;'
  containerRef.value.appendChild(canvas)

  const myConfetti = confetti.create(canvas, { resize: true, useWorker: true })
  const origin = {
    x: (rect.width / 2) / rect.width,
    y: 0.1,
  }

  myConfetti({
    particleCount: 120,
    spread: 80,
    origin,
    colors: ['#32CE13', '#D2F612', '#A8E63D', '#F0F9B3', '#ffffff'],
    scalar: 0.9,
  })

  setTimeout(() => canvas.remove(), 4000)
}

function maybeConfetti() {
  if (store.displayZone === 'Strong' || store.displayZone === 'Stable') {
    setTimeout(launchConfetti, 300)
  }
}

onMounted(maybeConfetti)
watch(() => store.isDiagnosing, (diagnosing) => {
  if (!diagnosing) maybeConfetti()
})
</script>
