// Enums as string literal unions

export type CategoryKey =
  | 'FoodDining'
  | 'Groceries'
  | 'BillsUtilities'
  | 'Transport'
  | 'Shopping'
  | 'HealthWellness'
  | 'Housing'
  | 'Transfers'
  | 'EntertainmentSubs'
  | 'Misc'

export type CategoryTier = 'Essential' | 'Discretionary' | 'Committed'

export type ScenarioType =
  | 'SpendingCut'
  | 'IncomeGain'
  | 'OneTimeInject'
  | 'HousingChange'
  | 'Custom'

export type EffortTag = 'Quick' | 'Habit' | 'Life'

export type Recurrence = 'OneTime' | 'Recurring'

export type ZoneName = 'Critical' | 'Fragile' | 'Stable' | 'Strong'

export type ArchetypeKey =
  | 'LifestyleInflator'
  | 'SteadySpender'
  | 'ResilientSaver'
  | 'CrisisMode'

// Core interfaces

export interface RunwayState {
  liquidCash: number
  monthlyBurn: number
  takeHome: number
  categories: Record<CategoryKey, number>
}

export interface MerchantSummary {
  name: string
  monthlyAvg: number
}

export interface CategoryBreakdownEntry {
  monthlyAverage: number
  monthlyAmounts: number[]
  tier: CategoryTier
  topMerchants: MerchantSummary[]
  transactionCount: number
}

export interface ScenarioParams {
  category?: CategoryKey
  cutPct?: number
  cutAmount?: number
  gainAmount?: number
  injectAmount?: number
  rentDelta?: number
  monthlyAmount?: number
  userLabel?: string
}

export interface Scenario {
  id: string
  type: ScenarioType
  label: string
  effort: EffortTag
  recurrence: Recurrence
  params: ScenarioParams
  assumption?: string | null
  delta: number
}

export interface ArchetypeInfo {
  key: ArchetypeKey
  name: string
  signal: string
}

export interface DangerSignal {
  severity: 'danger' | 'caution'
  title: string
  detail: string
  metric: string
  category?: CategoryKey | null
}

export interface TrendInfo {
  category: CategoryKey
  direction: 'growing' | 'stable' | 'declining'
  pctChange: number
  notable: boolean
  topMerchant: string
  topMerchantAmount: number
}

export interface InsightProfile {
  archetype: ArchetypeInfo
  dangerSignals: DangerSignal[]
  trends: TrendInfo[]
  remittanceNote?: string | null
  flexibleBurn: number
  fixedBurn: number
}

export interface DiagnosisContent {
  archetypeName: string
  whatIsHappening: string
  whatToDoAboutIt: string
  honestTake: string
}

export interface CorrectionCandidate {
  transaction: {
    id: string
    rawDesc: string
    amount: number
    category?: CategoryKey
    merchant?: string
  }
  assignedCategory: CategoryKey
  confidenceScore: number
  reason: string
}

export interface ZoneInfo {
  name: ZoneName
  colourToken: string
  description: string
}

// API Response types

export interface RunwayAnalyzeResponse {
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
}

export interface RunwayDiagnoseResponse {
  diagnosis: DiagnosisContent
}

export interface ScenarioWithDelta {
  id: string
  delta: number
  newDays: number
  newZone: ZoneName
}

export interface RunwayComputeScenariosResponse {
  baselineDays: number
  baselineZone: ZoneName
  stackedDays: number
  stackedDelta: number
  stackedZone: ZoneName
  stackedDate: string
  scenarioDeltas: ScenarioWithDelta[]
  reverseModeIds?: string[] | null
}
