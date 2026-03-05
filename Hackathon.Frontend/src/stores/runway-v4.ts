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
  PayrollSummary,
} from '../api/runway-v4-types'
import {
  analyzeRunwayV4,
  diagnoseRunwayV4,
  computeScenariosV4,
  fetchPayrollSummary,
} from '../api/runway-v4-client'

export const useRunwayV4Store = defineStore('runway-v4', () => {
  // ── State ──────────────────────────────────────────────────────────

  const currentScreen = ref(1)
  const liquidSavings = ref(0)
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
  const customScenarios = ref<Scenario[]>([])
  let nextCustomId = 1
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

  const payroll = ref<PayrollSummary | null>(null)
  const isLoadingPayroll = ref(false)

  // ── Computed ───────────────────────────────────────────────────────

  const fastestWin = computed(() =>
    scenarios.value.find(s => s.id === fastestWinId.value) ?? null,
  )

  const displayDays = computed(() =>
    activeScenarioIds.value.length > 0 && !isComputingScenarios.value
      ? stackedDays.value
      : baselineDays.value,
  )

  const displayZone = computed(() =>
    activeScenarioIds.value.length > 0 && !isComputingScenarios.value
      ? stackedZone.value
      : zone.value,
  )

  // ── Actions ────────────────────────────────────────────────────────

  async function analyze() {
    isAnalyzing.value = true
    error.value = null
    currentScreen.value = 4 // processing screen

    try {
      const savings = Number(liquidSavings.value) || 0
      const response = await analyzeRunwayV4(
        monthlyIncome.value,
        savings,
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
    customScenarios.value = []
    reverseTarget.value = null
    stackedDays.value = response.baselineDays
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

  async function setCustomScenario(label: string, monthlyAmount: number) {
    const id = `custom_${nextCustomId++}`
    const custom: Scenario = {
      id,
      type: 'Custom',
      label,
      effort: 'Habit',
      recurrence: 'Recurring',
      params: { monthlyAmount, userLabel: label },
      delta: 0,
    }
    customScenarios.value.push(custom)
    activeScenarioIds.value.push(id)
    await recomputeScenarios()
  }

  async function clearCustomScenario(id: string) {
    customScenarios.value = customScenarios.value.filter(s => s.id !== id)
    const index = activeScenarioIds.value.indexOf(id)
    if (index >= 0) {
      activeScenarioIds.value.splice(index, 1)
    }
    await recomputeScenarios()
  }

  async function setReverseTarget(target: number) {
    reverseTarget.value = target
    await recomputeScenarios()
  }

  async function recomputeScenarios() {
    if (!state.value) return

    isComputingScenarios.value = true
    error.value = null

    // Snapshot active state so we can revert on failure
    const prevActiveIds = [...activeScenarioIds.value]
    const prevStackedDays = stackedDays.value
    const prevStackedDelta = stackedDelta.value
    const prevStackedZone = stackedZone.value
    const prevStackedDate = stackedDate.value

    try {
      const response = await computeScenariosV4(
        state.value,
        [...scenarios.value, ...customScenarios.value],
        activeScenarioIds.value,
        null,
        reverseTarget.value,
      )
      applyScenarioResult(response)
    } catch (e: any) {
      error.value = e.message || 'Scenario computation failed'
      // Revert to previous stacked values so display doesn't show stale/zero data
      stackedDays.value = prevStackedDays
      stackedDelta.value = prevStackedDelta
      stackedZone.value = prevStackedZone
      stackedDate.value = prevStackedDate
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
      const custom = customScenarios.value.find(s => s.id === sd.id)
      if (custom) {
        custom.delta = sd.delta
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
    liquidSavings.value = 0
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
    customScenarios.value = []
    reverseTarget.value = null

    stackedDays.value = 0
    stackedDelta.value = 0
    stackedZone.value = 'Stable'
    stackedDate.value = ''
    reverseModeIds.value = []

    diagnosis.value = null

    payroll.value = null
    isLoadingPayroll.value = false

    isAnalyzing.value = false
    isDiagnosing.value = false
    isComputingScenarios.value = false
    error.value = null
  }

  function analyzeFromPayroll(manualSpend: number) {
    if (!payroll.value) return

    // Separate government statutory deductions from other deductions
    const govNames = ['SSS', 'PhilHealth', 'Pag-IBIG', 'Withholding Tax']
    const govDeductions = payroll.value.deductions.filter(d => govNames.includes(d.name))
    const otherDeductions = payroll.value.deductions.filter(d => !govNames.includes(d.name))

    const govTotal = govDeductions.reduce((sum, d) => sum + d.amount, 0)
    const otherDeductionTotal = otherDeductions.reduce((sum, d) => sum + d.amount, 0)
    const earningsTotal = payroll.value.earnings.reduce((sum, e) => sum + e.amount, 0)
    const deductionTotal = govTotal + otherDeductionTotal
    const monthlyBurn = manualSpend > 0 ? manualSpend : deductionTotal

    const govMerchants = govDeductions.map(d => ({ name: d.name, monthlyAvg: d.amount }))
    const otherMerchants = otherDeductions.map(d => ({ name: d.name, monthlyAvg: d.amount }))

    // Remaining spend after deductions goes to Misc (user's estimated discretionary spending)
    const remainingSpend = Math.max(0, monthlyBurn - deductionTotal)

    const emptyCat = (tier: CategoryBreakdownEntry['tier']): CategoryBreakdownEntry => ({
      monthlyAverage: 0, monthlyAmounts: [0], tier, topMerchants: [], transactionCount: 0,
    })

    const payrollCategories: Record<CategoryKey, CategoryBreakdownEntry> = {
      GovernmentDeductions: {
        monthlyAverage: govTotal,
        monthlyAmounts: [govTotal],
        tier: 'Committed' as const,
        topMerchants: govMerchants,
        transactionCount: govDeductions.length,
      },
      BillsUtilities: {
        monthlyAverage: otherDeductionTotal,
        monthlyAmounts: [otherDeductionTotal],
        tier: 'Essential' as const,
        topMerchants: otherMerchants,
        transactionCount: otherDeductions.length,
      },
      Misc: {
        monthlyAverage: remainingSpend,
        monthlyAmounts: [remainingSpend],
        tier: 'Discretionary' as const,
        topMerchants: [],
        transactionCount: 0,
      },
      FoodDining: emptyCat('Discretionary'),
      Groceries: emptyCat('Essential'),
      Transport: emptyCat('Essential'),
      Shopping: emptyCat('Discretionary'),
      HealthWellness: emptyCat('Essential'),
      Housing: emptyCat('Essential'),
      Transfers: emptyCat('Committed'),
      EntertainmentSubs: emptyCat('Discretionary'),
    }

    const takeHome = payroll.value.netPay + earningsTotal

    const runwayState: RunwayState = {
      liquidCash: Number(liquidSavings.value) || 0,
      monthlyBurn: monthlyBurn,
      takeHome: takeHome,
      categories: Object.fromEntries(
        Object.entries(payrollCategories).map(([k, v]) => [k, v.monthlyAverage])
      ) as Record<CategoryKey, number>,
    }

    // Compute baseline days
    let days: number
    if (runwayState.monthlyBurn <= 0) {
      days = 0
    } else if (runwayState.liquidCash > 0) {
      days = Math.floor(runwayState.liquidCash / (runwayState.monthlyBurn / 30))
    } else {
      // No savings: project 6 months of surplus as effective savings
      const monthlySurplus = runwayState.takeHome - runwayState.monthlyBurn
      if (monthlySurplus > 0) {
        const projectedCash = monthlySurplus * 6
        days = Math.floor(projectedCash / (runwayState.monthlyBurn / 30))
      } else {
        days = 0
      }
    }

    const getZone = (d: number): ZoneName => {
      if (d < 30) return 'Critical'
      if (d < 60) return 'Fragile'
      if (d < 120) return 'Stable'
      return 'Strong'
    }

    state.value = runwayState
    baselineDays.value = days
    zone.value = getZone(days)
    categories.value = payrollCategories
    monthlyIncome.value = takeHome
    insightProfile.value = {
      archetype: { key: 'SteadySpender', name: 'Steady Spender', signal: '' },
      dangerSignals: [],
      trends: [],
      remittanceNote: null,
      flexibleBurn: remainingSpend,
      fixedBurn: govTotal + otherDeductionTotal,
    }
    scenarios.value = []
    fastestWinId.value = null
    dangerSignals.value = []
    correctionCandidates.value = []
    analysisDate.value = new Date().toISOString()

    activeScenarioIds.value = []
    customScenarios.value = []
    reverseTarget.value = null
    stackedDays.value = days
    stackedDelta.value = 0
    stackedZone.value = zone.value
    stackedDate.value = ''

    currentScreen.value = 4
    setTimeout(() => { currentScreen.value = 5 }, 2500)
  }

  async function fetchPayroll() {
    if (payroll.value?.grossPay) return // already fetched with real data
    isLoadingPayroll.value = true
    const staticFallback = {
      grossPay: 75000,
      netPay: 65250,
      tax: 7487.5,
      payrollPeriod: 'Dec 16 - 31, 2025',
      employeeName: '',
      earnings: [
        { name: 'Basic Salary', amount: 37500 },
        { name: 'Transportation Allowance', amount: 2000 },
        { name: 'Rice Subsidy', amount: 1500 },
        { name: 'Overtime Pay', amount: 34000 },
      ],
      deductions: [
        { name: 'SSS Contribution', amount: 1125 },
        { name: 'PhilHealth', amount: 937.5 },
        { name: 'Pag-IBIG', amount: 200 },
        { name: 'Withholding Tax', amount: 7487.5 },
      ],
    }
    try {
      const result = await fetchPayrollSummary()
      payroll.value = result?.grossPay ? result : staticFallback
      monthlyIncome.value = payroll.value.netPay
    } catch {
      payroll.value = staticFallback
      monthlyIncome.value = staticFallback.netPay
    } finally {
      isLoadingPayroll.value = false
    }
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
    customScenarios,
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
    payroll,
    isLoadingPayroll,

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
    fetchPayroll,
    analyzeFromPayroll,
  }
})
