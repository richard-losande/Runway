<template>
  <div class="min-h-screen bg-gray-50">
    <InputScreen v-if="store.currentScreen === 1" />
    <ProcessingScreen v-else-if="store.currentScreen === 2" />
    <IntelligenceReport v-else-if="store.currentScreen === 3" />
    <SurvivalDashboard v-else-if="store.currentScreen === 4" />
    <DiagnosisScreen v-else-if="store.currentScreen === 5" />
  </div>
</template>

<script setup lang="ts">
import { useRunwayStore } from './stores/runway'
import InputScreen from './components/InputScreen.vue'
import ProcessingScreen from './components/ProcessingScreen.vue'
import IntelligenceReport from './components/IntelligenceReport.vue'
import SurvivalDashboard from './components/SurvivalDashboard.vue'
import DiagnosisScreen from './components/DiagnosisScreen.vue'

const store = useRunwayStore()
</script>
