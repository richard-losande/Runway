# Runway Superpowers — Full Rebuild Design

## Overview

Full rebuild of the Runway financial resilience engine to match the Superpowers PRD. The backend is restructured into 3 independent superpowers (SP1, SP2, SP3) with a core orchestrator. The frontend is a new Vue 3 + Vite app using the Sprout Design System with 5 sequential screens.

**Key decisions:**
- SP1 uses live OpenAI for transaction categorization (not hardcoded)
- Frontend calls ApiService directly (no BFF auth for demo)
- Sprout Design System (`design-system-next`) for all UI components
- Vue 3 + TypeScript + Vite + Pinia

---

## Backend Architecture

### Endpoints (ApiService)

| Endpoint | Route | Method | Purpose |
|---|---|---|---|
| Orchestrator | `POST /api/v1/runway/analyze` | POST | Entry point — runs SP1 → routes → SP2, returns combined result |
| Scenarios | `POST /api/v1/runway/scenarios` | POST | Re-runs SP2 with toggled scenarios (no SP1 re-run) |
| Profile | `POST /api/v1/runway/profile` | POST | Runs SP3 on demand ("Reveal My Profile" CTA) |

### Folder Structure

```
Features/Runway/
├── AnalyzeEndpoint.cs              -- Orchestrator endpoint
├── ScenariosEndpoint.cs            -- SP2 scenario toggle endpoint
├── ProfileEndpoint.cs              -- SP3 profile endpoint
├── Services/
│   ├── ITransactionIntelligence.cs -- SP1 interface
│   ├── TransactionIntelligence.cs  -- SP1: OpenAI categorization
│   ├── ISurvivalSimulator.cs       -- SP2 interface
│   ├── SurvivalSimulator.cs        -- SP2: deterministic calculations
│   ├── IBehavioralIntelligence.cs  -- SP3 interface
│   ├── BehavioralIntelligence.cs   -- SP3: archetype + OpenAI diagnosis
│   ├── IRunwayOrchestrator.cs      -- Orchestrator interface
│   └── RunwayOrchestrator.cs       -- Routing logic
├── Models/
│   ├── Sp1Models.cs                -- SP1 input/output DTOs
│   ├── Sp2Models.cs                -- SP2 input/output DTOs
│   ├── Sp3Models.cs                -- SP3 input/output DTOs
│   └── OrchestratorModels.cs       -- Combined request/response DTOs
└── DemoData/
    └── AlexTransactions.csv        -- Demo CSV with 4 months of data
```

### SP1 — Transaction Intelligence Engine (OpenAI-powered)

**Input:**
```json
{
  "transactions": [{ "date": "...", "description": "...", "amount": 0, "type": "debit" }],
  "monthly_income": 75000,
  "months_covered": 4
}
```

**Processing:** Sends CSV content to OpenAI with a structured prompt to:
- Categorize each transaction into Fixed / Variable / Discretionary
- Compute monthly burn and burn breakdown
- Calculate elasticity score: `(variable + discretionary) / monthly_burn`
- Identify top 2 danger signals with monthly growth rates
- Determine `top_danger_category`

**Output:** Matches PRD Section 8 output schema exactly.

**Fallback:** On OpenAI failure, returns hardcoded Alex demo data.

### SP2 — Survival & Scenario Simulator (Deterministic)

**No OpenAI calls.** Pure math.

**Baseline:**
```
survival_days = (liquid_savings / monthly_burn) * 30
monthly_surplus = monthly_income - monthly_burn
burn_rate = monthly_burn / monthly_income
```

**Scenarios:**

| ID | Delta Rule |
|---|---|
| `cut_lifestyle` | burn -= discretionary * 0.70 |
| `side_hustle` | income += 10,000 |
| `salary_increase` | income += income * 0.10 |
| `major_upgrade` | burn += 15,000 |

**Stacking:** Cumulative in toggle order. Max 3 active.

**Stability zones:** Critical (<30), Fragile (30–59), Stable (60–119), Strong (120+).

**Output:** Matches PRD Section 9 output schema exactly.

### SP3 — Behavioral Intelligence (Deterministic + OpenAI)

**Archetype pre-classification (deterministic):**
```
IF elasticity_score > 0.50 AND income_to_burn_ratio < 1.20 → "Lifestyle Inflator"
IF elasticity_score < 0.30 AND income_to_burn_ratio > 1.50 → "Stability Builder"
IF danger_signals[0].monthly_growth_rate > 0.30 → "Spending Accelerator"
DEFAULT → "Balanced Spender"
```

**OpenAI call:** Uses the exact prompt contract from PRD Section 10.

**Fallback:** Hardcoded diagnosis per archetype (from PRD) on LLM failure.

**Output:** `{ archetype, diagnosis, top_recommendation, closing_line }`

### Orchestrator Routing Decision

```csharp
string Route(string topDangerCategory, double incomeToBurnRatio)
{
    if (new[] { "Dining", "Shopping", "Entertainment" }.Contains(topDangerCategory))
        return "cut_lifestyle";
    if (incomeToBurnRatio < 1.10)
        return "side_hustle";
    return "cut_lifestyle";
}
```

---

## Frontend Architecture

### Project: `Hackathon.Frontend/`

Vue 3 + TypeScript + Vite with:
- `design-system-next` (Sprout Design System)
- Pinia for state management
- Axios for API calls

### State (Pinia)

Single `useRunwayStore`:
- `currentScreen` (1–5)
- `userInputs` (income, savings, csvFile)
- `sp1Result` (burn profile, danger signals)
- `sp2Result` (baseline, scenarios, stacked result)
- `sp3Result` (archetype, diagnosis)
- `activeScenarios` (string[], max 3)
- `isLoading`, `error`

### Screen 1 — Input

- Pre-filled monthly income (₱75,000, read-only `spr-input`)
- Fixed deductions summary (read-only)
- Liquid savings (`spr-input-currency`)
- CSV upload (`spr-file-upload`) + "Use Demo Data" (`spr-button`)
- Privacy note
- CTA: "Show Me My Runway →" — calls `POST /api/v1/runway/analyze`

### Screen 2 — Processing

- "Analyzing your transactions..." header
- Animated categorization feed (timed, line by line)
- Auto-transitions to Screen 3 when API response arrives

### Screen 3 — Intelligence Report

- Horizontal stacked bar: Fixed / Variable / Lifestyle burn breakdown
- Total monthly burn figure
- 2 danger signal cards (amber-styled `spr-card`)
- CTA: "See My Survival Days →"

### Screen 4 — Survival Dashboard

- Large survival days number (animated counter)
- Human label ("That's about 3 months and 13 days")
- Stability zone `spr-badge` (Rose / Amber / Yellow / Emerald)
- 4-zone legend
- 4 scenario toggle cards (`spr-card` + `spr-switch`), priority scenario first
- On toggle: `POST /api/v1/runway/scenarios` with active scenarios
- Max 3 enforcement with friendly inline message
- CTA: "Reveal My Profile →"

### Screen 5 — Behavioral Diagnosis

- Archetype name (large heading)
- Diagnosis paragraph (numbers highlighted)
- Top recommendation (boxed `spr-card`)
- Closing line (italic, muted)
- "Try Different Numbers" restart → Screen 1

### Navigation

Single-page flow. No Vue Router. Screens advance via CTAs. Only Screen 5 loops back.

---

## Data Flow

```
Screen 1 (user inputs)
    → POST /api/v1/runway/analyze
        → SP1 (OpenAI categorization)
        → Routing decision
        → SP2 (deterministic baseline + scenarios)
    ← { sp1Result, sp2Result }

Screen 2 (processing animation, waiting for response)

Screen 3 (show sp1Result: burn breakdown + danger signals)

Screen 4 (show sp2Result: survival days + scenario toggles)
    → POST /api/v1/runway/scenarios (on toggle)
    ← { updated sp2Result with stacked scenarios }

Screen 5 (triggered by "Reveal My Profile")
    → POST /api/v1/runway/profile
        → Deterministic archetype classification
        → SP3 (OpenAI diagnosis)
    ← { sp3Result }
```

---

## What Gets Removed

The existing `Features/FinancialRunway/` folder (AnalyzeEndpoint, OpenAiService, Models) is replaced by the new `Features/Runway/` structure. The old single-endpoint approach is fully superseded.

---

## Out of Scope

- Keycloak / JWT authentication
- BFF proxy layer
- Database persistence (can be added post-demo)
- Mobile responsiveness (desktop demo only)
- Real CSV parsing from bank files (demo CSV only)
- Aspire AppHost changes for frontend (served standalone by Vite)
