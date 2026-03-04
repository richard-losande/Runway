# Payroll Integration Design

## Goal

Connect to the Sprout payroll API from the BFF via Refit, display payroll breakdown on Screen 2 (PayrollProfile), and auto-populate `takeHome` for runway calculations.

## Data Flow

```
Screen 1 (PayFeedScreen) mounts
  → axios GET to BFF: /api/v1/payroll/summary
  → BFF calls Sprout payroll API via Refit (ISproutPayrollClient)
  → BFF transforms response → PayrollSummaryResponse
  → Frontend stores in Pinia (runway-v4 store)
  → netAmount → store.takeHome (auto-populates for runway)
  → Screen 2 (PayrollProfile) reads from store, displays breakdown
```

## BFF Changes

### Refit Interface

New file: `Hackathon.Bff/Integrations/SproutPayroll/ISproutPayrollClient.cs`

- GET `/api/v1/payrolls/entries/{entryId}/summary?search={search}`
- Required headers:
  - `x-api-key` — from appsettings (`SproutPayroll:ApiKey`)
  - `x-payroll-PayrollPieDatabase: Payroll_GC03` — hardcoded
  - `Accept: application/json`
- Base URL from appsettings (`SproutPayroll:BaseUrl`)
- DelegatingHandler to inject headers

### BFF Endpoint

New file: `Hackathon.Bff/Features/Payroll/SummaryEndpoint.cs`

- Route: GET `/api/v1/payroll/summary`
- No request params (hardcoded: entry ID `1002`, search `LM_Feaure_TestData`)
- Calls `ISproutPayrollClient`, transforms to simplified DTO

### Response DTO

```csharp
PayrollSummaryResponse:
  GrossPay: decimal        // basicSalary from API
  NetPay: decimal          // netAmount from API (raw, no recalculation)
  Tax: decimal             // tax from API
  Deductions: List<PayrollLineItem>  // SSS (sssee), PhilHealth (phee), Pag-IBIG (hdmfee), negative adjustments
  Earnings: List<PayrollLineItem>    // positive adjustments
  EmployeeName: string     // firstName + lastName
  PayrollPeriod: string    // e.g. "January 2026 - Period 1"

PayrollLineItem:
  Name: string
  Amount: decimal
```

### Deduction/Earnings Logic

- `sssee` → "SSS" deduction
- `phee` → "PhilHealth" deduction
- `hdmfee` → "Pag-IBIG" deduction
- `tax` → "Withholding Tax" deduction
- Adjustments with `amount > 0` → earnings
- Adjustments with `amount < 0` → deductions (absolute value)

### Configuration

In `appsettings.Development.json`:
```json
{
  "SproutPayroll": {
    "BaseUrl": "https://app-spr-payrollapi-v3-qa.azurewebsites.net",
    "ApiKey": "x7#hx$Yz@Z6!CB5fxrZWXw5Jeo$7eSA219tqiy5Gu29ktsj6X8"
  }
}
```

Hardcoded in handler: `x-payroll-PayrollPieDatabase: Payroll_GC03`

## Frontend Changes

### Pinia Store (`runway-v4.ts`)

- New state: `payroll: PayrollSummary | null`
- New action: `fetchPayroll()` — calls BFF, stores result, sets `takeHome = netPay`
- Loading/error state for payroll fetch

### PayFeedScreen.vue (Screen 1)

- On mount: call `store.fetchPayroll()`
- Show loading state while fetching

### PayrollProfile.vue (Screen 2)

- Replace all hardcoded values with `store.payroll` data
- Earnings section: gross pay + positive adjustments
- Deductions section: SSS, PhilHealth, Pag-IBIG, tax, negative adjustments
- Net Take-Home: netPay from store

## Decisions

- **Refit client lives in BFF** (not ApiService) — BFF calls Sprout directly
- **Use raw `netAmount`** from API as net pay — no recalculation
- **Hardcoded params** for hackathon: entry ID 1002, search "LM_Feaure_TestData"
- **Base URL + API key in config**, database header hardcoded
- **Fetch once on Screen 1 mount**, data persists in Pinia for all screens
- **`netAmount` auto-populates `takeHome`** for runway calculations
