// === SP1 ===
export interface DangerSignal {
  category: string
  monthlyGrowthRate: number
  monthlyAmount: number
  insight: string
}

export interface BurnBreakdown {
  fixed: number
  variable: number
  discretionary: number
}

export interface Sp1Output {
  monthlyBurn: number
  burnBreakdown: BurnBreakdown
  elasticityScore: number
  incomeToBurnRatio: number
  dangerSignals: DangerSignal[]
  topDangerCategory: string
}

// === SP2 ===
export interface BaselineResult {
  survivalDays: number
  humanLabel: string
  monthlyBurn: number
  monthlySurplus: number
  burnRate: number
  stabilityZone: string
}

export interface ScenarioResult {
  id: string
  label: string
  survivalDays: number
  deltaDays: number
  isPriority: boolean
}

export interface StackedResult {
  activeScenarios: string[]
  survivalDays: number
  deltaDays: number
}

export interface Sp2Output {
  baseline: BaselineResult
  scenarios: ScenarioResult[]
  stackedResult: StackedResult
}

// === SP3 ===
export interface Sp3Output {
  archetype: string
  diagnosis: string
  topRecommendation: string
  closingLine: string
}

// === API Responses ===
export interface AnalyzeResponse {
  sp1: Sp1Output
  sp2: Sp2Output
}
