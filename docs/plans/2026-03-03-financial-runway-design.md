# Financial Runway Simulator — Design

## Overview

A single-endpoint financial runway analyzer that accepts a bank statement CSV, salary, savings, and planned life events, then uses OpenAI GPT-5 to parse transactions, calculate runway, simulate life event impacts, and return both structured data and narrative advice. Results are stored per-user in PostgreSQL.

## API Contract

### `POST /api/v1/financial-runway/analyze`

**Request** (multipart form):
- `csvFile` — bank statement CSV file upload
- `monthlySalary` — decimal, recurring monthly income
- `totalSavings` — decimal, current savings balance
- `lifeEvents` — JSON array:
  - `type` — enum: `BuyHouse`, `BuyCar`, `HaveBaby`, `LoseJob`, `GetRaise`, `Custom`
  - `description` — free-text (required for Custom, optional for predefined)
  - `monthFromNow` — int, when the event occurs

**Response** (JSON):
- `runwayMonths` — months until money runs out (base, no life events)
- `monthlyBurnRate` — average monthly spending
- `categorizedExpenses` — `[{ category, monthlyAverage, percentage }]`
- `monthlyProjections` — `[{ month, balance, income, expenses }]` (12-month forecast)
- `lifeEventImpacts` — `[{ event, impactOnRunway, newMonthlyExpense }]`
- `adjustedRunwayMonths` — runway after life events applied
- `narrative` — OpenAI-generated analysis and advice
- `analyzedAt` — datetime

## Architecture

```
Client → BFF (POST /api/v1/financial-runway/analyze)
         → Refit → ApiService (POST /api/v1/financial-runway/analyze)
                    → Read CSV content
                    → Build OpenAI prompt (CSV + salary + savings + life events)
                    → Call OpenAI GPT-5 (structured JSON output)
                    → Map response to DTO
                    → Save to PostgreSQL (FinancialAnalysis table)
                    → Return response
         ← Return to client
```

### Where things live

**ApiService** — `Features/FinancialRunway/`:
- `AnalyzeEndpoint.cs` — FastEndpoint, receives request, orchestrates flow
- `OpenAiService.cs` — wraps OpenAI HTTP call, builds prompt, parses response
- `Models.cs` — request/response DTOs and DB entity

**ApiService DB** — new `FinancialAnalysis` entity in `MainDbContext` + EF migration

**BFF** — new Refit method on `IApiServiceClient` + new FastEndpoint proxy

**OpenAI** — called via HttpClient from ApiService using the REST API directly. API key in ApiService's appsettings under `OpenAI:ApiKey`.

## OpenAI Prompt Strategy

Single system + user message call to GPT-5 with `response_format: { type: "json_object" }`.

**System message:** "You are a financial analyst. Given a bank statement CSV, monthly salary, savings, and planned life events, produce a structured JSON analysis of the user's financial runway."

**User message contains:**
1. Raw CSV content
2. Monthly salary and total savings
3. Life events list
4. JSON schema instruction for the response

**OpenAI is asked to:**
- Parse and categorize all CSV transactions
- Calculate monthly burn rate from recent transaction history
- Compute base runway (savings / net monthly burn)
- Simulate each life event's financial impact
- Project 12 months with and without life events
- Write a narrative summary with actionable advice

## Decisions

- Single OpenAI call (not multi-stage) — fastest for hackathon
- ApiService owns OpenAI integration and data storage
- BFF is a thin proxy
- Life events support both predefined types and free-form custom events
- Response includes both structured data (for UI/charts) and narrative text
- Analysis history stored per user in PostgreSQL
