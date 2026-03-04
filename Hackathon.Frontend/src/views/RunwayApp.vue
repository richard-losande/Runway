<template>
  <div class="min-h-screen bg-gray-50">
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
import { useRunwayV4Store } from '../stores/runway-v4'
import PayFeedScreen from '../components/v4/PayFeedScreen.vue'
import PayrollProfile from '../components/v4/PayrollProfile.vue'
import DataConnection from '../components/v4/DataConnection.vue'
import ProcessingScreenV4 from '../components/v4/ProcessingScreenV4.vue'
import IntelligenceReportV4 from '../components/v4/IntelligenceReportV4.vue'
import SurvivalDashboardV4 from '../components/v4/SurvivalDashboardV4.vue'
import DiagnosisScreenV4 from '../components/v4/DiagnosisScreenV4.vue'
import ActionCard from '../components/v4/ActionCard.vue'

const store = useRunwayV4Store()
</script>
