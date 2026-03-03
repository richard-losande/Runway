import axios from 'axios'
import type { AnalyzeResponse, Sp2Output, Sp3Output, BurnBreakdown, DangerSignal, ScenarioResult } from './types'

const api = axios.create({
  baseURL: 'http://localhost:5407',
  headers: { 'Content-Type': 'application/json' },
})

export async function analyzeRunway(
  monthlyIncome: number,
  liquidSavings: number,
  csvFile: File | null,
  useDemoData: boolean,
): Promise<AnalyzeResponse> {
  const formData = new FormData()
  formData.append('monthlyIncome', monthlyIncome.toString())
  formData.append('liquidSavings', liquidSavings.toString())
  formData.append('useDemoData', useDemoData.toString())
  if (csvFile) {
    formData.append('csvFile', csvFile)
  }

  const { data } = await api.post<AnalyzeResponse>('/api/v1/runway/analyze', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return data
}

export async function recalculateScenarios(
  monthlyBurn: number,
  burnBreakdown: BurnBreakdown,
  monthlyIncome: number,
  liquidSavings: number,
  priorityScenario: string,
  activeScenarios: string[],
): Promise<Sp2Output> {
  const { data } = await api.post<Sp2Output>('/api/v1/runway/scenarios', {
    monthlyBurn,
    burnBreakdown,
    monthlyIncome,
    liquidSavings,
    priorityScenario,
    activeScenarios,
  })
  return data
}

export async function revealProfile(
  elasticityScore: number,
  incomeToBurnRatio: number,
  dangerSignals: DangerSignal[],
  baselineSurvivalDays: number,
  topScenario: ScenarioResult,
  burnBreakdown: BurnBreakdown,
  monthlyBurn: number,
): Promise<Sp3Output> {
  const { data } = await api.post<Sp3Output>('/api/v1/runway/profile', {
    elasticityScore,
    incomeToBurnRatio,
    dangerSignals,
    baselineSurvivalDays,
    topScenario,
    burnBreakdown,
    monthlyBurn,
  })
  return data
}
