# Runway v4 Spec Implementation Design

## Summary

Full rewrite of the Runway financial resilience calculator to match the v4 product spec. Replaces the current 5-screen/3-endpoint implementation with the spec's 8-screen architecture, 4-agent system, typed scenario model, zone system, and demo mode.

## Architecture

### Backend (ApiService — .NET 10, FastEndpoints)

**Computation Engine** (`Services/RunwayEngine.cs`):
- `ComputeBaseline(RunwayState)` → runway days
- `ComputeScenarioDays(Scenario, RunwayState)` → days with scenario applied
- `ComputeDelta(Scenario, RunwayState)` → delta from baseline
- `FindFastestPath(targetDays, RunwayState, Scenario[])` → greedy scenario selection
- `GetZone(days)` → Critical/Fragile/Stable/Strong
- `DaysToDate(days, referenceDate)` → calendar date string
- All pure functions, fully unit-testable

**Pipeline Services** (transaction processing):
- `FormatDetector` — Stage 1: identify bank CSV format (GCash, BDO, BPI, UnionBank, Maya, RCBC)
- `TransactionNormalizer` — Stage 2: parse, normalize, exclude non-spend rows
- `MerchantLookup` — Stage 3: rule-based pattern matching against seed list (~200 patterns)
- `OpenAiFallback` — Stage 4: gpt-4o-mini batch categorization for unresolved transactions
- `Aggregator` — Stage 5: group by category × month, compute averages, flag low-confidence

**Agent Services**:
- `CategorizationAgent` (Agent 1) — gpt-4o-mini, stubbed in demo mode
- `InsightExtractor` (Agent 2) — gpt-4o, stubbed in demo mode → InsightProfile
- `ScenarioGenerator` (Agent 3) — gpt-4o-mini, stubbed in demo mode → Scenario[]
- `DiagnosisNarrative` (Agent 4) — gpt-4o, LIVE in demo mode → DiagnosisContent

**Endpoints**:
- `POST /api/v1/runway/analyze` — CSV upload → full pipeline → CategoryBreakdown + RunwayState + InsightProfile + Scenario[] + baseline + zone
- `POST /api/v1/runway/diagnose` — InsightProfile + RunwayState → DiagnosisContent (live Agent 4 call)
- `POST /api/v1/runway/compute-scenarios` — active scenario IDs + custom scenario → stacked result (new days, delta, zone, date)

### Frontend (Vue 3 + Pinia + TOGE design system)

**8 Screens** (display + input only, no computation):
1. PayFeedScreen — entry card on payslip
2. PayrollProfile — pre-filled payroll data + savings input
3. DataConnection — choose GCash / CSV / Estimate
4. ProcessingScreen — animated 3.2s pipeline visualization
5. IntelligenceReport — burn breakdown + danger signals + corrections
6. SurvivalDashboard — runway number + zone bar + scenario chips + reverse mode
7. DiagnosisScreen — archetype card + live AI narrative + attribution
8. ActionCard — zone-routed product card (ReadySave/ReadyCash/ReadyWage)

**State Management** (Pinia store):
- Holds all runway state received from backend
- Calls backend for all computations (scenario toggles → POST compute-scenarios)
- No computation logic in frontend

## Data Models

All models match the spec exactly:

### Core Types
- `RunwayState` — liquidCash, monthlyBurn, takeHome, categories (9 categories + misc)
- `CategoryBreakdown` — per-category: monthlyAverage, monthlyAmounts[], tier, topMerchants[], transactionCount
- `Transaction` — date, amount, rawDesc, normDesc, source, category, merchant, confidence

### Scenario System
- `Scenario` — id, type, label, effort, recurrence, params, assumption
- Types: SPENDING_CUT, INCOME_GAIN, ONE_TIME_INJECT, HOUSING_CHANGE, CUSTOM
- Effort tags: quick, habit, life

### Zone System
- Critical: < 30 days (TOMATO/red)
- Fragile: 30–59 days (MANGO/yellow)
- Stable: 60–119 days (BLUEBERRY/blue)
- Strong: 120+ days (KANGKONG/green)

### Agent Outputs
- `InsightProfile` — archetype, dangerSignals[], trends[], remittanceNote, flexibleBurn, fixedBurn
- `DiagnosisContent` — archetypeName, whatIsHappening (280 chars), whatToDoAboutIt (240 chars), honestTake (180 chars)

## Demo Mode

- `DEMO_MODE` is a backend environment variable (build-time constant)
- Agents 1–3 return fixture data (Alex Garcia dataset from spec section 12.5)
- Agent 4 makes live gpt-4o call with 10s timeout, pre-written fallback on failure
- Frontend Screen 4 shows 6 demo transaction strings + category fade-in animation
- All fixture data matches spec section 12.5 exactly (liquidCash=180,000, monthlyBurn=52,400, etc.)

## Component Mapping (TOGE)

Uses spr-* components throughout:
- `spr-button` — all CTAs (tone: success/neutral, variant: primary/secondary/tertiary)
- `spr-card` — content containers
- `spr-chips` — scenario chips (Screen 6), amount selector (Screen 8)
- `spr-progress-bar` — processing (Screen 4), burn breakdown (Screen 5)
- `spr-badge` — inline labels (brand, information, danger, pending, caution)
- `spr-status` — danger signals (Screen 5)
- `spr-input` — savings input, estimate input, custom scenario, reverse mode target
- `spr-collapsible` — diagnosis text (Screen 7)
- `spr-logo` — product logos (Screen 8)

## YAGNI Boundaries (from spec)

Not building:
- Scenario conflict detection
- Employer-facing dashboard
- Multi-currency support
- Effort tag filter UI (tags present, filter deferred)
- Optimal subset-sum for reverse mode (greedy only)
- Push notifications
- Savings goal tracking over time
