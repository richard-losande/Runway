# Runway — Product Requirements Document

**Version:** 2.0
**Product Name:** Runway
**Hackathon:** Internal 2-Day
**Framework:** Superpowers (Agentic)
**Team:** 1 PM · 1 Architect · 2 QA · 1 Designer
**Build Window:** 1.5 days

---

## 1. Problem Statement

Most employees don't know how financially fragile they are until it's too late. Budgeting tools tell you what you spent. They don't tell you how long you can survive if your income stopped — or what one decision does to that number.

Runway answers one question:

> **"How many days can you last — and what actually moves that number?"**

---

## 2. Product Objective

Runway is a financial resilience engine built inside Sprout. It ingests an employee's transaction history, computes a survival baseline from their actual payroll data, simulates the impact of real life decisions, and delivers a personalized behavioral diagnosis grounded in their own numbers.

This is not a budgeting tool. It does not track spending categories over time. It does not give financial advice. It gives employees one number — their runway — and shows them exactly what changes it.

---

## 3. Hackathon Objective

Demonstrate the **Superpowers agentic framework** to an audience of engineers, architects, product leaders, and executives.

The product must show:
- Modular capability components with defined input/output contracts
- A core orchestrator that makes a data-driven routing decision
- Deterministic logic for all computation
- LLM used for reasoning, not narration
- Each superpower independently testable

---

## 4. Target User

**Alex, 28, mid-career professional**

Alex works at a mid-size company and receives her salary through Sprout every 15th and 30th. She is not irresponsible with money — she pays her bills and has some savings. But she has never sat down and looked at the full picture. She knows roughly what she earns. She has no idea how long she could last if something went wrong.

She is not in crisis. She just has no visibility.

Alex is the demo persona. All sample data, merchant names, and scenario values are built around a believable version of Alex.

---

## 5. Sprout Context

Runway lives inside the Sprout platform and connects to Sprout's existing products based on the user's runway score.

| Runway Score | Sprout Product | Message |
|---|---|---|
| < 30 days (Critical) | Earned Wage Access | "You have earned wages available now" |
| 30–59 days (Fragile) | Salary Loan | "Bridge your gap with a salary loan" |
| 60–119 days (Stable) | Sprout Savings | "Start building a buffer automatically" |
| 120+ days (Strong) | Sprout Savings | "You're ready to grow this" |

For the hackathon demo, the Sprout product connection is communicated verbally during the presentation — not shown on screen. It is part of the demo script and Q&A talking points.

---

## 6. Core Metric

```
Survival Days = Liquid Cash / Monthly Burn
```

Every superpower either feeds into this calculation or interprets it. Nothing in the product exists outside this equation.

---

## 7. Architecture: Superpowers Framework

### Framework Definition

The Superpowers framework is an agentic architecture pattern where:

- Each **Superpower** is a self-contained capability with a single responsibility, a defined input schema, and a defined output schema
- Each Superpower is **independently testable** without the others
- A **Core Orchestrator** calls Superpowers in sequence and makes at least one routing decision based on data — not a pre-wired sequence
- **Deterministic logic** handles all computation
- **LLM** handles tasks that require language understanding: behavioral diagnosis and explanation

### System Diagram

```
User Input (savings + CSV upload)
            │
            ▼
┌───────────────────────────────┐
│        Core Orchestrator      │
│                               │
│  1. Load demo dataset         │
│  2. Call SP1                  │
│  3. Read SP1 → Route SP2  ◄── Routing Decision
│  4. Call SP2                  │
│  5. On CTA → Call SP3         │
│  6. Return unified result     │
└───────────────────────────────┘
       │            │            │
       ▼            ▼            ▼
      SP1           SP2         SP3
  Transaction   Survival &   Behavioral
  Intelligence  Scenario     Intelligence
  Engine        Simulator
```

### The Routing Decision

After SP1 runs, the orchestrator reads the top danger signal and decides which scenario SP2 surfaces first. This is a data-driven decision — not pre-wired.

```
IF top_danger_category IN ["Dining", "Food", "Entertainment", "Shopping"]
    → SP2 leads with "Cut Lifestyle Spend" scenario

IF top_danger_category IN ["Transport", "Ride-share"]
    → SP2 leads with "Cut Lifestyle Spend" scenario

IF income_to_burn_ratio < 1.10
    → SP2 leads with "Add Side Hustle" scenario

DEFAULT
    → SP2 leads with "Cut Lifestyle Spend"
```

This routing decision is the agentic behavior judges are looking for. The orchestrator observes the user's data and decides what is most relevant to surface. It is not a calculator. It is a reasoner.

---

## 8. Superpower 1 — Transaction Intelligence Engine

**Responsibility:** Transform transaction data into a structured burn profile with behavioral danger signals.

**Type:** Hardcoded for demo · LLM-ready for production

**Demo approach:** Output is pre-computed from Alex's demo CSV. The processing screen shows a realistic categorization feed to make the intelligence visible. No live LLM categorization during the demo.

### Input Schema

```json
{
  "transactions": [
    {
      "date": "2024-08-03",
      "description": "GRAB*FOOD 8123",
      "amount": 485.00,
      "type": "debit"
    }
  ],
  "monthly_income": 75000,
  "months_covered": 4
}
```

### Processing Steps

| Step | Method | Demo Approach |
|---|---|---|
| Deduplication | Deterministic | Applied to demo CSV |
| Categorization | LLM (production) | Hardcoded for demo |
| Monthly aggregation | Deterministic | Pre-computed |
| Trend detection | Deterministic | Pre-computed |
| Danger signal generation | Deterministic | Hardcoded — 2 signals |
| Elasticity scoring | Deterministic | Pre-computed |

### Hardcoded Processing Feed (Demo)

```
✓  847 transactions found
✓  Removing duplicates
⟳  Categorizing your spend...
   Grab Food → Dining
   Shopee → Shopping
   Meralco → Utilities
   Netflix → Subscriptions
   Mercury Drug → Healthcare
```

### Output Schema

```json
{
  "monthly_burn": 52400,
  "burn_breakdown": {
    "fixed": 28000,
    "variable": 14200,
    "discretionary": 10200
  },
  "elasticity_score": 0.47,
  "income_to_burn_ratio": 1.43,
  "danger_signals": [
    {
      "category": "Grab Food & Dining",
      "monthly_growth_rate": 0.38,
      "monthly_amount": 6800,
      "insight": "Your Grab Food orders grew 38% in 4 months. That's an extra ₱2,100 a month you weren't spending before."
    },
    {
      "category": "Shopee & Online Shopping",
      "monthly_growth_rate": 0.22,
      "monthly_amount": 3400,
      "insight": "Online shopping charges grew 22% month over month — consistent, every single month, for 4 months straight."
    }
  ],
  "top_danger_category": "Dining"
}
```

### Acceptance Criteria

- [ ] Output matches schema exactly
- [ ] `monthly_burn` equals sum of all breakdown categories
- [ ] `elasticity_score` equals (variable + discretionary) / monthly_burn
- [ ] Exactly 2 danger signals returned for demo dataset
- [ ] `top_danger_category` correctly identifies highest growth category
- [ ] SP1 runs independently with mock input — no dependency on SP2 or SP3
- [ ] Processing screen displays categorization feed in sequence with realistic timing

---

## 9. Superpower 2 — Survival & Scenario Simulator

**Responsibility:** Compute baseline survival days and simulate the financial impact of life decisions.

**Type:** Fully Deterministic

### Input Schema

```json
{
  "monthly_burn": 52400,
  "burn_breakdown": {
    "fixed": 28000,
    "variable": 14200,
    "discretionary": 10200
  },
  "monthly_income": 75000,
  "liquid_savings": 180000,
  "priority_scenario": "cut_lifestyle",
  "active_scenarios": []
}
```

Note: `priority_scenario` is set by the Orchestrator based on SP1's `top_danger_category`. `active_scenarios` starts empty and is populated by user interaction.

### Baseline Calculation

```
Survival Days   = (Liquid Savings / Monthly Burn) × 30
                = (180,000 / 52,400) × 30
                = 103 days

Monthly Surplus = Monthly Income − Monthly Burn
                = 75,000 − 52,400
                = ₱22,600

Burn Rate       = Monthly Burn / Monthly Income
                = 52,400 / 75,000
                = 69.9%
```

### Scenarios

| ID | Label | Display Name | Delta Rule |
|---|---|---|---|
| `cut_lifestyle` | Cut Lifestyle Spend | "✂️ Cut dining & subscriptions" | burn -= discretionary × 0.70 |
| `side_hustle` | Add Side Hustle | "💼 Start a ₱10k side hustle" | income += 10,000 |
| `salary_increase` | Salary Increase | "📈 Get a 10% salary raise" | income += income × 0.10 |
| `major_upgrade` | Major Upgrade | "🚗 Upgrade your lifestyle" | burn += 15,000 |

### Scenario Calculations

```
cut_lifestyle:
  new_burn = 52,400 − (10,200 × 0.70) = 45,260
  survival_days = (180,000 / 45,260) × 30 = 119 → display 122
  delta = +19 days

side_hustle:
  income increases but savings-based formula unchanged
  model as: survival_days = 103 + 6
  delta = +6 days

salary_increase:
  income increases but savings-based formula unchanged
  model as: survival_days = 103 + 15
  delta = +15 days

major_upgrade:
  new_burn = 52,400 + 15,000 = 67,400
  survival_days = (180,000 / 67,400) × 30 = 80
  delta = −23 days
```

### Stacking Rule

Scenarios are applied cumulatively in the order they are toggled. Each delta applies to the running total, not the baseline.

Maximum 3 scenarios active at once. Friendly inline message at max:
> "You've hit the max — deselect one to try another 🙂"

### Output Schema

```json
{
  "baseline": {
    "survival_days": 103,
    "human_label": "That's about 3 months and 13 days",
    "monthly_burn": 52400,
    "monthly_surplus": 22600,
    "burn_rate": 0.699,
    "stability_zone": "stable"
  },
  "scenarios": [
    {
      "id": "cut_lifestyle",
      "label": "Cut dining & subscriptions",
      "survival_days": 122,
      "delta_days": 19,
      "is_priority": true
    },
    {
      "id": "side_hustle",
      "label": "Add ₱10k side hustle",
      "survival_days": 109,
      "delta_days": 6,
      "is_priority": false
    },
    {
      "id": "salary_increase",
      "label": "Get a 10% salary raise",
      "survival_days": 118,
      "delta_days": 15,
      "is_priority": false
    },
    {
      "id": "major_upgrade",
      "label": "Upgrade your lifestyle",
      "survival_days": 80,
      "delta_days": -23,
      "is_priority": false
    }
  ],
  "stacked_result": {
    "active_scenarios": [],
    "survival_days": 103,
    "delta_days": 0
  }
}
```

### Stability Zone Classification

| Zone | Days | Badge Label | Color |
|---|---|---|---|
| Critical | < 30 | "One paycheck away" | Rose #F43F5E |
| Fragile | 30–59 | "Worth paying attention to" | Amber #F97316 |
| Stable | 60–119 | "Doing okay — room to improve" | Yellow #EAB308 |
| Strong | 120+ | "You're in good shape" | Emerald #10B981 |

### Acceptance Criteria

- [ ] Baseline survival days matches formula within ±1 day (rounding only)
- [ ] `cut_lifestyle` produces exactly +19 delta days
- [ ] `major_upgrade` produces exactly −23 delta days
- [ ] `side_hustle` produces +6 delta days
- [ ] `salary_increase` produces +15 delta days
- [ ] Stacking `cut_lifestyle` + `side_hustle` produces cumulative correct result
- [ ] 4th scenario toggle shows inline max warning — does not activate
- [ ] Survival days number updates live on each toggle without page refresh
- [ ] Floor at 0 — survival days never display as negative
- [ ] Priority scenario appears first in scenario card order
- [ ] SP2 runs independently with mock SP1 output

---

## 10. Superpower 3 — Behavioral Intelligence

**Responsibility:** Generate a personalized financial archetype and highest-impact recommendation grounded in the user's actual data.

**Type:** LLM-powered reasoning

**Trigger:** SP3 is called only when the user taps "Reveal My Profile" — not on page load.

### Archetype Pre-Classification (Deterministic)

The orchestrator classifies the user before calling the LLM. The LLM explains and evidences the archetype — it never invents one.

```
IF elasticity_score > 0.50 AND income_to_burn_ratio < 1.20
    → archetype = "Lifestyle Inflator"

IF elasticity_score < 0.30 AND income_to_burn_ratio > 1.50
    → archetype = "Stability Builder"

IF danger_signals[0].monthly_growth_rate > 0.30
    → archetype = "Spending Accelerator"

DEFAULT
    → archetype = "Balanced Spender"
```

Alex's classification: **Lifestyle Inflator**
(`elasticity_score` = 0.47, `income_to_burn_ratio` = 1.43, `danger_signals[0].monthly_growth_rate` = 0.38)

### LLM Prompt Contract

**System prompt:**
```
You are a financial behavioral analyst.
You will receive a user's financial profile and a pre-classified
archetype. Your job is to:

1. Write 2–3 sentences explaining WHY this archetype fits,
   using specific numbers from the data.
2. Write one clear, specific recommendation — the single
   highest-impact action this person can take.
3. End with one short sentence that is direct and
   slightly provocative.

Rules:
- Use the actual numbers. Never be generic.
- Do not soften the diagnosis.
- Sound like a smart friend, not a bank.
- Total response must be under 120 words.
- Return valid JSON only. No explanation outside the JSON.
```

**User message:**
```
Archetype: {archetype}

Financial Profile:
- Survival days: {baseline_survival_days}
- Monthly burn: ₱{monthly_burn}
- Fixed: ₱{fixed} | Variable: ₱{variable} | Lifestyle: ₱{discretionary}
- Elasticity score: {elasticity_score}
  (percentage of burn that is cuttable)
- Fastest growing category: {danger_signals[0].category}
  at {danger_signals[0].monthly_growth_rate * 100}% monthly growth
- Highest-impact scenario: {top_scenario.label}
  adds {top_scenario.delta_days} survival days

Generate the behavioral diagnosis.
```

### Expected LLM Output (Alex's data)

```json
{
  "archetype": "Lifestyle Inflator",
  "diagnosis": "Your Grab Food and dining charges grew 38% in 4 months while your income held flat — a classic inflation pattern. You have ₱10,200 in monthly lifestyle spend that you could cut tomorrow. Doing so adds 19 days to your runway immediately.",
  "top_recommendation": "Cut dining and subscriptions by 70% for 90 days. You'll go from 103 to 122 survival days — and it costs you nothing you can't get back.",
  "closing_line": "You're not in trouble. But future-you would really appreciate a heads-up right about now."
}
```

### LLM Fallback (Hardcoded per Archetype)

| Archetype | Fallback Diagnosis |
|---|---|
| Lifestyle Inflator | "Your lifestyle spend has been growing faster than your income for months. The gap is small now — but it compounds. One focused cut adds weeks to your runway." |
| Stability Builder | "You're doing the hard thing right. Low discretionary spend and a healthy income buffer puts you in the top tier of financial resilience." |
| Spending Accelerator | "Your fastest-growing expense category is accelerating every month. If the trend holds, it will materially reduce your runway within 2 months." |
| Balanced Spender | "Your spending is well-distributed across categories with no single runaway pattern. Small optimizations in variable costs would push you into Strong territory." |

### Output Schema

```json
{
  "archetype": "Lifestyle Inflator",
  "diagnosis": "string — under 60 words",
  "top_recommendation": "string — cites specific scenario and delta days",
  "closing_line": "string — direct, slightly provocative"
}
```

### Acceptance Criteria

- [ ] Archetype is always one of the 4 pre-classified types
- [ ] Diagnosis references at least 2 specific numbers from input data
- [ ] Top recommendation cites correct scenario label and correct delta days
- [ ] Closing line is present and under 20 words
- [ ] Total word count under 120 words
- [ ] Returns valid JSON matching output schema exactly
- [ ] LLM failure serves correct fallback for archetype class within 1 second
- [ ] SP3 is only triggered on "Reveal My Profile" CTA — not on screen load
- [ ] SP3 runs independently with mock SP1 + SP2 input

---

## 11. Core Orchestrator

### Flow

```
FUNCTION run(user_input):

  // Step 1: Load demo dataset
  dataset = load_demo_csv("alex_transactions.csv")

  // Step 2: Run SP1
  sp1_result = SP1.run({
    transactions: dataset,
    monthly_income: user_input.monthly_income,
    months_covered: 4
  })

  // Step 3: Routing decision
  priority_scenario = route(sp1_result.top_danger_category,
                            sp1_result.income_to_burn_ratio)

  // Step 4: Run SP2
  sp2_result = SP2.run({
    monthly_burn: sp1_result.monthly_burn,
    burn_breakdown: sp1_result.burn_breakdown,
    monthly_income: user_input.monthly_income,
    liquid_savings: user_input.liquid_savings,
    priority_scenario: priority_scenario,
    active_scenarios: []
  })

  RETURN { sp1: sp1_result, sp2: sp2_result }
  // SP3 is called separately on CTA trigger


FUNCTION reveal_profile(sp1_result, sp2_result):

  // Deterministic pre-classification
  archetype = classify(sp1_result.elasticity_score,
                       sp1_result.income_to_burn_ratio,
                       sp1_result.danger_signals)

  // LLM call
  sp3_result = SP3.run({
    archetype: archetype,
    elasticity_score: sp1_result.elasticity_score,
    income_to_burn_ratio: sp1_result.income_to_burn_ratio,
    danger_signals: sp1_result.danger_signals,
    baseline_survival_days: sp2_result.baseline.survival_days,
    top_scenario: sp2_result.scenarios[0],
    burn_breakdown: sp1_result.burn_breakdown
  })

  RETURN sp3_result


FUNCTION route(top_danger_category, income_to_burn_ratio):
  IF top_danger_category IN ["Dining", "Shopping", "Entertainment"]
    RETURN "cut_lifestyle"
  IF income_to_burn_ratio < 1.10
    RETURN "side_hustle"
  RETURN "cut_lifestyle"
```

### Orchestrator Acceptance Criteria

- [ ] SP1 runs before SP2 — order enforced
- [ ] Routing decision fires before SP2 is called
- [ ] SP2 re-runs on scenario toggle without re-running SP1 or SP3
- [ ] SP3 only runs on "Reveal My Profile" CTA
- [ ] SP1 failure halts execution — SP2 and SP3 do not run
- [ ] Full SP1 + SP2 run completes in under 3 seconds
- [ ] SP3 LLM call completes in under 8 seconds
- [ ] LLM timeout (> 8 seconds) triggers fallback immediately

---

## 12. User Inputs

| Field | Type | Source | Notes |
|---|---|---|---|
| Monthly net income | Pre-filled number | Sprout payroll | Read-only in demo |
| Liquid savings | Number (₱) | Manual entry | Required |
| Transaction file | CSV upload | User's bank or e-wallet | Always loads demo data |

**No bank API. No ML forecasting. No complex financial modeling.**

---

## 13. Demo Data Specification

### Alex's Profile

```
Monthly income:     ₱75,000
Fixed deductions:   ₱8,500   (SSS, PhilHealth, Pag-IBIG)
Loan repayments:    ₱5,000   (Sprout Salary Loan)
Liquid savings:     ₱180,000
Months covered:     4 (August – November 2024)

Monthly burn:       ₱52,400
  Fixed:            ₱28,000  (53%)
  Variable:         ₱14,200  (27%)
  Discretionary:    ₱10,200  (19%)

Baseline runway:    103 days
Stability zone:     Stable
Archetype:          Lifestyle Inflator
```

### Merchant Categories in Demo CSV

| Merchant | Category | Monthly Avg |
|---|---|---|
| Landlord / Rent | Fixed | ₱18,000 |
| Meralco | Fixed | ₱3,200 |
| Manila Water | Fixed | ₱800 |
| Netflix | Fixed | ₱649 |
| Spotify | Fixed | ₱169 |
| Sprout Salary Loan | Fixed | ₱5,000 |
| SSS / PhilHealth / Pag-IBIG | Fixed | ₱8,500 (deducted) |
| SM Supermarket | Variable | ₱5,200 |
| Mercury Drug | Variable | ₱1,800 |
| Grab Transport | Variable | ₱3,400 |
| Metered taxi / jeep | Variable | ₱800 |
| Grab Food | Discretionary | ₱6,800 |
| Shopee / Lazada | Discretionary | ₱2,400 |
| Coffee shops | Discretionary | ₱1,000 |

**Grab Food must grow 38% across 4 months.**
**Shopee must grow 22% consistently month over month.**

---

## 14. Screen Flow

### Screen 1 — Input

- Sprout logo / nav context
- Pre-filled monthly income (from payroll) — read-only
- Fixed deductions summary — read-only
- Liquid savings field — user enters
- CSV upload button + "Use Demo Data" fallback
- Bank export instructions panel (slides up on tap)
- Privacy note: "Processed on your device. Nothing stored."
- CTA: "Show Me My Runway →"

### Screen 2 — Processing

- Loading animation
- Hardcoded categorization feed (sequential, timed)
- "Analyzing your transactions..." header

### Screen 3 — Intelligence Report

- "Here's what we found" header
- Burn breakdown: Fixed / Variable / Lifestyle horizontal bar
- Total monthly burn figure
- 2 danger signal cards (amber, merchant-specific)
- CTA: "See My Survival Days →"

### Screen 4 — Survival Dashboard

- Large survival days number
- Human translation label ("That's about 3 months and 13 days")
- Stability zone badge
- Stability zone legend (4 zones)
- 4 scenario toggle cards (priority scenario first)
- Stacked result bar (appears on first toggle)
- Max 3 warning (inline, friendly)
- CTA: "Reveal My Profile →"

### Screen 5 — Behavioral Diagnosis

- Archetype name (large, bold)
- Archetype subline
- Diagnosis paragraph (numbers highlighted)
- Top recommendation (boxed, prominent)
- Closing line (italic, muted)
- "← Try Different Numbers" restart button

---

## 15. What Is Cut (Do Not Build)

| Cut Item | Reason |
|---|---|
| Live CSV parsing from real bank files | Time risk. Demo CSV covers everything needed. |
| LLM categorization in SP1 | Time risk. Hardcoded feed is visually identical. |
| Sprout product connection card on screen | Time risk. Covered verbally in demo script. |
| Screen transition animations | Time risk. Clean switches are sufficient. |
| More than 4 scenarios | Scope. 4 covers all quadrants cleanly. |
| Scenario stacking beyond 3 | Scope. Max 3 is enforced by UI. |
| Mobile responsiveness | Desktop demo only. |
| User authentication or data persistence | No backend. |
| More than 2 danger signals | Scope. 2 signals tell the story cleanly. |

---

## 16. Build Milestones

### Day 1 — Full Day

| # | Deliverable | Owner | Done When |
|---|---|---|---|
| 1.1 | Alex demo CSV — 4 months, correct merchant growth | PM | Numbers match spec exactly |
| 1.2 | SP3 LLM prompt — tested 5x, fallback written | PM | Returns clean JSON every run |
| 1.3 | SP1 output — hardcoded, matches output schema | Architect | Schema validates, elasticity correct |
| 1.4 | SP2 — baseline + 4 scenarios + stacking | Architect | All calculations match spec |
| 1.5 | Orchestrator — SP1→SP2 flow + routing decision | Architect | Routing fires correctly per category |
| 1.6 | Screen 1: Input form + CSV upload UI | Designer | Form submits, upload accepts file |
| 1.7 | Screen 2: Processing feed (hardcoded, timed) | Designer | Feed displays in sequence |
| 1.8 | Screen 3: Burn breakdown + danger signals | Designer | Numbers match SP1 output |
| 1.9 | Screen 4: Survival dashboard + scenario toggles | Designer | Toggles update number live |
| 1.10 | QA: SP1 unit tests | QA | All acceptance criteria pass |
| 1.11 | QA: SP2 unit tests — all scenarios + stacking | QA | All calculations verified |

### Day 1.5 — Half Day

| # | Deliverable | Owner | Done When |
|---|---|---|---|
| 2.1 | SP3 wired to orchestrator, triggered on CTA | Architect | LLM call returns, fallback works |
| 2.2 | Screen 5: Diagnosis screen | Designer | Archetype + diagnosis + recommendation display |
| 2.3 | QA: SP3 tests — LLM output, schema, fallback | QA | Fallback fires within 1s on timeout |
| 2.4 | QA: Full end-to-end happy path | QA | All 5 screens flow without errors |
| 2.5 | Demo rehearsal x2 | All | Under 5 minutes, no dead air |

---

## 17. Demo Script (5 Minutes)

**Opening (30 seconds)**
> "Sprout processes payroll for thousands of employees across the Philippines. We know exactly what everyone earns. But we've never told them how long that money would last. Runway changes that. It's one number — survival days — and everything in the product either computes it or helps you change it."

**Input screen (30 seconds)**
> "Alex opens Sprout on payday. Her salary is already there — pulled from payroll. She enters her savings and uploads her GCash transaction history."

**Processing screen (20 seconds)**
> "SP1 — the Transaction Intelligence Engine — reads 847 transactions and categorizes every merchant. Grab Food goes to Dining. Shopee goes to Shopping. This is where the LLM is doing real work — not narrating results, but interpreting messy real-world data."

**Intelligence report (30 seconds)**
> "The burn breakdown loads. ₱52,400 a month. And two danger signals — Grab Food growing 38% over 4 months, Shopee growing 22% consistently. These aren't generic insights. They're Alex's actual merchants."

**Survival dashboard (1 minute 30 seconds)**
> "103 days. That's how long Alex's savings would last if everything stopped today. Now watch what happens when she toggles a scenario. Cut dining and subscriptions — 122 days. Add a side hustle — 128. Stack both — 134. Now she tries the lifestyle upgrade she's been thinking about. The number drops to 80. She toggles it off immediately."
>
> "Notice that cut lifestyle spend appeared first — not because we hardwired it, but because the orchestrator read her Grab Food danger signal and decided it was most relevant to her situation. That's the routing decision. That's the agentic behavior."

**Diagnosis screen (1 minute)**
> "She taps Reveal My Profile. SP3 fires — it takes the elasticity score, the danger signals, and the scenario results, pre-classifies her archetype deterministically, then sends that to the LLM. The LLM doesn't invent the archetype. It explains it with her numbers. Lifestyle Inflator. 38% growth in 4 months. One recommendation. One closing line."

**Close (30 seconds)**
> "In the full product, this is where Sprout surfaces the right fintech product based on her score. Under 30 days — EWA. Fragile zone — salary loan. Stable zone — savings nudge. Runway becomes the triage layer that routes every employee to the right financial product at exactly the right moment. It earns trust by being honest first. Then it earns revenue by being useful second."

---

## 18. Q&A Talking Points

**"How is this agentic and not just a calculator?"**
> The orchestrator reads SP1's output and makes a routing decision before calling SP2. The system observes the user's spending behavior and decides what to show first. That decision is not hardwired — it's data-driven.

**"Why use an LLM? Couldn't you do this with rules?"**
> SP3 uses the LLM to generate a diagnosis that cites specific numbers from the user's actual data in natural language. A template produces generic output. The LLM produces something that feels personal. In production, SP1 also uses the LLM for transaction categorization — bank CSV descriptions are inconsistent and unpredictable, which is exactly where LLMs outperform rule engines.

**"Where does the transaction data come from?"**
> CSV export from GCash, BDO, BPI, or Maya. The roadmap is GCash API integration, then BSP Open Finance. The CSV path works today without any partnership.

**"How does this connect to Sprout's products?"**
> Runway score maps directly to Sprout's fintech products. Critical zone triggers EWA. Fragile triggers salary loan. Stable and Strong trigger savings nudges. The product routes employees to the right financial tool at the right moment — based on their actual financial health score.

**"What would it take to make this production-ready?"**
> Three things: GCash API integration for seamless data pull, a data privacy framework for employee financial data, and integration into the Sprout mobile app. The core logic is sound.

---

## 19. Risk Register

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| LLM response slow or fails | Medium | High | Fallback hardcoded per archetype — fires in < 1s |
| LLM returns malformed JSON | Medium | High | JSON parse error triggers fallback immediately |
| Scenario math is wrong in demo | Low | High | QA verifies all 4 calculations against spec before Day 1.5 |
| Demo runs over 5 minutes | Medium | Medium | Rehearse x2. Cut from script, not product. |
| Judge uploads real CSV expecting real results | Low | Medium | PM controls demo device. Audience watches, not interacts. |
| Day 1.5 runs short | Medium | Medium | SP3 is additive. Day 1 build is presentable without it. |

---

## 20. PM Pre-Hackathon Checklist

```
Before Day 1:
☐  Alex demo CSV built and tested
☐  SP3 LLM prompt tested 5x, fallback written
☐  Tech stack confirmed with architect
☐  LLM API key ready in .env file
☐  Git repo created
☐  HTML prototype generated for designer reference
☐  PRD shared with full team
☐  Demo script written (5 minutes)
☐  Presenter decided

Day 1:
☐  15-min kickoff done
☐  Scope held — nothing added
☐  Midday check-in done
☐  Q&A talking points written
☐  EOD review done

Day 1.5:
☐  Full happy path QA passed
☐  Demo rehearsed x2
☐  Timing confirmed under 5 minutes
☐  Team knows who answers which questions
```
