import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Sp1Output, Sp2Output, Sp3Output } from '../api/types'
import { analyzeRunway, recalculateScenarios, revealProfile } from '../api/client'

export const useRunwayStore = defineStore('runway', () => {
  // State
  const currentScreen = ref(1)
  const monthlyIncome = ref(75000)
  const liquidSavings = ref(180000)
  const csvFile = ref<File | null>(null)
  const useDemoData = ref(true)

  const sp1Result = ref<Sp1Output | null>(null)
  const sp2Result = ref<Sp2Output | null>(null)
  const sp3Result = ref<Sp3Output | null>(null)

  const activeScenarios = ref<string[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Computed
  const priorityScenario = computed(() =>
    sp2Result.value?.scenarios.find(s => s.isPriority)?.id ?? 'cut_lifestyle'
  )

  // Actions
  async function analyze() {
    isLoading.value = true
    error.value = null
    currentScreen.value = 2 // Show processing screen

    try {
      const result = await analyzeRunway(
        monthlyIncome.value,
        liquidSavings.value,
        csvFile.value,
        useDemoData.value,
      )
      sp1Result.value = result.sp1
      sp2Result.value = result.sp2
      activeScenarios.value = []
      currentScreen.value = 3 // Show intelligence report
    } catch (e: any) {
      error.value = e.message || 'Analysis failed'
      currentScreen.value = 1 // Go back to input
    } finally {
      isLoading.value = false
    }
  }

  async function toggleScenario(scenarioId: string) {
    if (!sp1Result.value || !sp2Result.value) return

    const index = activeScenarios.value.indexOf(scenarioId)
    if (index >= 0) {
      activeScenarios.value.splice(index, 1)
    } else {
      if (activeScenarios.value.length >= 3) return // Max 3
      activeScenarios.value.push(scenarioId)
    }

    try {
      sp2Result.value = await recalculateScenarios(
        sp1Result.value.monthlyBurn,
        sp1Result.value.burnBreakdown,
        monthlyIncome.value,
        liquidSavings.value,
        priorityScenario.value,
        [...activeScenarios.value],
      )
    } catch (e: any) {
      error.value = e.message || 'Scenario calculation failed'
    }
  }

  async function revealMyProfile() {
    if (!sp1Result.value || !sp2Result.value) return

    isLoading.value = true
    error.value = null

    try {
      const topScenario = sp2Result.value.scenarios[0]!
      sp3Result.value = await revealProfile(
        sp1Result.value.elasticityScore,
        sp1Result.value.incomeToBurnRatio,
        sp1Result.value.dangerSignals,
        sp2Result.value.baseline.survivalDays,
        topScenario,
        sp1Result.value.burnBreakdown,
        sp1Result.value.monthlyBurn,
      )
      currentScreen.value = 5
    } catch (e: any) {
      error.value = e.message || 'Profile analysis failed'
    } finally {
      isLoading.value = false
    }
  }

  function goToScreen(screen: number) {
    currentScreen.value = screen
  }

  function restart() {
    currentScreen.value = 1
    sp1Result.value = null
    sp2Result.value = null
    sp3Result.value = null
    activeScenarios.value = []
    error.value = null
    liquidSavings.value = 180000
  }

  return {
    currentScreen,
    monthlyIncome,
    liquidSavings,
    csvFile,
    useDemoData,
    sp1Result,
    sp2Result,
    sp3Result,
    activeScenarios,
    isLoading,
    error,
    priorityScenario,
    analyze,
    toggleScenario,
    revealMyProfile,
    goToScreen,
    restart,
  }
})
