import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type {
  RunwayState,
  ZoneName,
  CategoryKey,
  CategoryBreakdownEntry,
  InsightProfile,
  Scenario,
  DangerSignal,
  CorrectionCandidate,
  DiagnosisContent,
} from '../api/runway-v4-types'
import {
  analyzeRunwayV4,
  diagnoseRunwayV4,
  computeScenariosV4,
} from '../api/runway-v4-client'

export const useRunwayV4Store = defineStore('runway-v4', () => {
  // ── State ──────────────────────────────────────────────────────────

  const currentScreen = ref(1)
  const liquidSavings = ref(180000)
  const monthlyIncome = ref(28500)

  const csvFile = ref<File | null>(null)
  const useDemoData = ref(true)

  const state = ref<RunwayState | null>(null)
  const baselineDays = ref(0)
  const zone = ref<ZoneName>('Stable')

  const categories = ref<Record<CategoryKey, CategoryBreakdownEntry> | null>(null)
  const insightProfile = ref<InsightProfile | null>(null)

  const scenarios = ref<Scenario[]>([])
  const fastestWinId = ref<string | null>(null)

  const dangerSignals = ref<DangerSignal[]>([])
  const correctionCandidates = ref<CorrectionCandidate[]>([])
  const analysisDate = ref('')

  const activeScenarioIds = ref<string[]>([])
  const customScenario = ref<Scenario | null>(null)
  const reverseTarget = ref<number | null>(null)

  const stackedDays = ref(0)
  const stackedDelta = ref(0)
  const stackedZone = ref<ZoneName>('Stable')
  const stackedDate = ref('')
  const reverseModeIds = ref<string[]>([])

  const diagnosis = ref<DiagnosisContent | null>(null)

  const isAnalyzing = ref(false)
  const isDiagnosing = ref(false)
  const isComputingScenarios = ref(false)
  const error = ref<string | null>(null)

  // ── Computed ───────────────────────────────────────────────────────

  const fastestWin = computed(() =>
    scenarios.value.find(s => s.id === fastestWinId.value) ?? null,
  )

  const displayDays = computed(() =>
    activeScenarioIds.value.length > 0 ? stackedDays.value : baselineDays.value,
  )

  const displayZone = computed(() =>
    activeScenarioIds.value.length > 0 ? stackedZone.value : zone.value,
  )

  // ── Actions ────────────────────────────────────────────────────────

  async function analyze() {
    isAnalyzing.value = true
    error.value = null
    currentScreen.value = 4 // processing screen

    try {
      const response = await analyzeRunwayV4(
        monthlyIncome.value,
        liquidSavings.value,
        csvFile.value,
        useDemoData.value,
      )
      applyAnalyzeResult(response)
      currentScreen.value = 5
    } catch (e: any) {
      error.value = e.message || 'Analysis failed'
      currentScreen.value = 1
    } finally {
      isAnalyzing.value = false
    }
  }

  function applyAnalyzeResult(response: {
    state: RunwayState
    baselineDays: number
    zone: ZoneName
    categories: Record<CategoryKey, CategoryBreakdownEntry>
    insightProfile: InsightProfile
    scenarios: Scenario[]
    fastestWinId?: string | null
    dangerSignals: DangerSignal[]
    correctionCandidates: CorrectionCandidate[]
    analysisDate: string
  }) {
    state.value = response.state
    baselineDays.value = response.baselineDays
    zone.value = response.zone
    categories.value = response.categories
    insightProfile.value = response.insightProfile
    scenarios.value = response.scenarios
    fastestWinId.value = response.fastestWinId ?? null
    dangerSignals.value = response.dangerSignals
    correctionCandidates.value = response.correctionCandidates
    analysisDate.value = response.analysisDate

    // Reset scenario selections
    activeScenarioIds.value = []
    customScenario.value = null
    reverseTarget.value = null
    stackedDays.value = 0
    stackedDelta.value = 0
    stackedZone.value = response.zone
    stackedDate.value = ''
  }

  async function toggleScenario(id: string) {
    const index = activeScenarioIds.value.indexOf(id)
    if (index >= 0) {
      activeScenarioIds.value.splice(index, 1)
    } else {
      activeScenarioIds.value.push(id)
    }
    await recomputeScenarios()
  }

  function setCustomScenario(label: string, monthlyAmount: number) {
    const custom: Scenario = {
      id: 'custom',
      type: 'Custom',
      label,
      effort: 'Habit',
      recurrence: 'Recurring',
      params: { monthlyAmount, userLabel: label },
      delta: 0,
    }
    customScenario.value = custom

    if (!activeScenarioIds.value.includes('custom')) {
      activeScenarioIds.value.push('custom')
    }
    recomputeScenarios()
  }

  function clearCustomScenario() {
    customScenario.value = null
    const index = activeScenarioIds.value.indexOf('custom')
    if (index >= 0) {
      activeScenarioIds.value.splice(index, 1)
    }
    recomputeScenarios()
  }

  function setReverseTarget(target: number) {
    reverseTarget.value = target
    recomputeScenarios()
  }

  async function recomputeScenarios() {
    if (!state.value) return

    isComputingScenarios.value = true
    error.value = null

    try {
      const response = await computeScenariosV4(
        state.value,
        scenarios.value,
        activeScenarioIds.value,
        customScenario.value,
        reverseTarget.value,
      )
      applyScenarioResult(response)
    } catch (e: any) {
      error.value = e.message || 'Scenario computation failed'
    } finally {
      isComputingScenarios.value = false
    }
  }

  function applyScenarioResult(response: {
    stackedDays: number
    stackedDelta: number
    stackedZone: ZoneName
    stackedDate: string
    scenarioDeltas: Array<{ id: string; delta: number }>
    reverseModeIds?: string[] | null
  }) {
    stackedDays.value = response.stackedDays
    stackedDelta.value = response.stackedDelta
    stackedZone.value = response.stackedZone
    stackedDate.value = response.stackedDate

    // Update individual scenario deltas from the response
    for (const sd of response.scenarioDeltas) {
      const scenario = scenarios.value.find(s => s.id === sd.id)
      if (scenario) {
        scenario.delta = sd.delta
      }
      if (customScenario.value && customScenario.value.id === sd.id) {
        customScenario.value.delta = sd.delta
      }
    }

    // Apply reverse mode results — auto-activate recommended scenarios
    if (response.reverseModeIds && response.reverseModeIds.length > 0) {
      activeScenarioIds.value = [...response.reverseModeIds]
      reverseModeIds.value = response.reverseModeIds
    }
  }

  async function fetchDiagnosis() {
    if (!insightProfile.value || !state.value) return

    isDiagnosing.value = true
    error.value = null

    try {
      const fw = fastestWin.value
      const response = await diagnoseRunwayV4(
        insightProfile.value,
        state.value,
        baselineDays.value,
        zone.value,
        fw?.label ?? '',
        fw?.delta ?? 0,
        fw ? baselineDays.value + fw.delta : baselineDays.value,
      )
      diagnosis.value = response.diagnosis
      currentScreen.value = 7
    } catch (e: any) {
      error.value = e.message || 'Diagnosis failed'
    } finally {
      isDiagnosing.value = false
    }
  }

  function goToScreen(n: number) {
    currentScreen.value = n
  }

  function restart() {
    currentScreen.value = 1
    liquidSavings.value = 180000
    monthlyIncome.value = 28500
    csvFile.value = null
    useDemoData.value = true

    state.value = null
    baselineDays.value = 0
    zone.value = 'Stable'

    categories.value = null
    insightProfile.value = null

    scenarios.value = []
    fastestWinId.value = null

    dangerSignals.value = []
    correctionCandidates.value = []
    analysisDate.value = ''

    activeScenarioIds.value = []
    customScenario.value = null
    reverseTarget.value = null

    stackedDays.value = 0
    stackedDelta.value = 0
    stackedZone.value = 'Stable'
    stackedDate.value = ''
    reverseModeIds.value = []

    diagnosis.value = null

    isAnalyzing.value = false
    isDiagnosing.value = false
    isComputingScenarios.value = false
    error.value = null
  }

  // ── Return ─────────────────────────────────────────────────────────

  return {
    // State
    currentScreen,
    liquidSavings,
    monthlyIncome,
    csvFile,
    useDemoData,
    state,
    baselineDays,
    zone,
    categories,
    insightProfile,
    scenarios,
    fastestWinId,
    dangerSignals,
    correctionCandidates,
    analysisDate,
    activeScenarioIds,
    customScenario,
    reverseTarget,
    stackedDays,
    stackedDelta,
    stackedZone,
    stackedDate,
    reverseModeIds,
    diagnosis,
    isAnalyzing,
    isDiagnosing,
    isComputingScenarios,
    error,

    // Computed
    fastestWin,
    displayDays,
    displayZone,

    // Actions
    analyze,
    applyAnalyzeResult,
    toggleScenario,
    setCustomScenario,
    clearCustomScenario,
    setReverseTarget,
    recomputeScenarios,
    applyScenarioResult,
    fetchDiagnosis,
    goToScreen,
    restart,
  }
})
