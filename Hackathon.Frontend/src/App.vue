<template>
<<<<<<< Updated upstream
  <router-view />
</template>
=======
  <div class="min-h-screen bg-gray-50">
    <!-- Navigation bar -->
    <div class="max-w-md mx-auto pt-4 px-4">
      <div class="flex items-center gap-3">
        <button
          v-if="store.currentScreen > 1"
          class="w-8 h-8 flex items-center justify-center rounded-full hover:bg-gray-200 transition-colors text-gray-600"
          @click="goBack"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
          </svg>
        </button>
        <div v-else class="w-8" />
        <StepIndicator :current="store.currentScreen" class="flex-1" />
      </div>
    </div>

    <!-- Screen content -->
    <PayFeedScreen v-if="store.currentScreen === 1" />
    <PayrollProfile v-else-if="store.currentScreen === 2" />
    <DataConnection v-else-if="store.currentScreen === 3" />
    <ProcessingScreenV4 v-else-if="store.currentScreen === 4" />
    <IntelligenceReportV4 v-else-if="store.currentScreen === 5" />
    <SurvivalDashboardV4 v-else-if="store.currentScreen === 6" />
    <DiagnosisScreenV4 v-else-if="store.currentScreen === 7" />
    <ActionCard v-else-if="store.currentScreen === 8" />
  </div>
</template>

<script setup lang="ts">
import { useRunwayV4Store } from './stores/runway-v4'
import StepIndicator from './components/v4/StepIndicator.vue'
import PayFeedScreen from './components/v4/PayFeedScreen.vue'
import PayrollProfile from './components/v4/PayrollProfile.vue'
import DataConnection from './components/v4/DataConnection.vue'
import ProcessingScreenV4 from './components/v4/ProcessingScreenV4.vue'
import IntelligenceReportV4 from './components/v4/IntelligenceReportV4.vue'
import SurvivalDashboardV4 from './components/v4/SurvivalDashboardV4.vue'
import DiagnosisScreenV4 from './components/v4/DiagnosisScreenV4.vue'
import ActionCard from './components/v4/ActionCard.vue'

const store = useRunwayV4Store()

function goBack() {
  store.goToScreen(store.currentScreen - 1)
}
</script>
>>>>>>> Stashed changes
