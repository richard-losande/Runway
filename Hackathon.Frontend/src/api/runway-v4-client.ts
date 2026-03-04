import axios from 'axios'
import type {
  RunwayAnalyzeResponse,
  RunwayDiagnoseResponse,
  RunwayComputeScenariosResponse,
  RunwayState,
  Scenario,
  InsightProfile,
  ZoneName,
  PayrollSummary,
} from './runway-v4-types'

const api = axios.create({
  headers: { 'Content-Type': 'application/json' },
})

export async function analyzeRunwayV4(
  monthlyIncome: number,
  liquidSavings: number,
  csvFile: File | null,
  useDemoData: boolean,
): Promise<RunwayAnalyzeResponse> {
  const formData = new FormData()
  formData.append('monthlyIncome', monthlyIncome.toString())
  formData.append('liquidSavings', liquidSavings.toString())
  formData.append('useDemoData', useDemoData.toString())
  if (csvFile) {
    formData.append('csvFile', csvFile)
  }
  const { data } = await api.post<RunwayAnalyzeResponse>(
    '/api/v1/runway-v4/analyze',
    formData,
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data
}

export async function diagnoseRunwayV4(
  insightProfile: InsightProfile,
  state: RunwayState,
  baselineDays: number,
  zone: ZoneName,
  fastestWinLabel: string,
  fastestWinDelta: number,
  fastestWinNewDays: number,
): Promise<RunwayDiagnoseResponse> {
  const { data } = await api.post<RunwayDiagnoseResponse>(
    '/api/v1/runway-v4/diagnose',
    {
      insightProfile,
      state,
      baselineDays,
      zone,
      fastestWinLabel,
      fastestWinDelta,
      fastestWinNewDays,
    },
  )
  return data
}

export async function computeScenariosV4(
  state: RunwayState,
  scenarios: Scenario[],
  activeScenarioIds: string[],
  customScenario?: Scenario | null,
  reverseTarget?: number | null,
): Promise<RunwayComputeScenariosResponse> {
  const { data } = await api.post<RunwayComputeScenariosResponse>(
    '/api/v1/runway-v4/compute-scenarios',
    {
      state,
      scenarios,
      activeScenarioIds,
      customScenario: customScenario ?? null,
      reverseTarget: reverseTarget ?? null,
    },
  )
  return data
}

export async function fetchPayrollSummary(): Promise<PayrollSummary> {
  const { data } = await api.get<PayrollSummary>('/api/v1/payroll/summary')
  return data
}
