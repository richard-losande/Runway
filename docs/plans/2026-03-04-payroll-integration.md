# Payroll Integration Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Connect BFF to Sprout payroll API via Refit, display payroll breakdown on Screen 2 (PayrollProfile), and auto-populate takeHome for runway calculations.

**Architecture:** BFF gets a new Refit client (`ISproutPayrollClient`) that calls the external Sprout payroll API. A new FastEndpoints GET endpoint (`/api/v1/payroll/summary`) calls it and returns a simplified DTO. The Vue frontend fetches this on Screen 1 mount, stores it in Pinia, and PayrollProfile (Screen 2) renders the live data.

**Tech Stack:** Refit 10.0.1, FastEndpoints 8.0.1, Pinia, axios, Vue 3

---

### Task 1: Create Sprout Payroll Refit Client and DelegatingHandler

**Files:**
- Create: `Hackathon.Bff/Integrations/SproutPayroll/ISproutPayrollClient.cs`
- Create: `Hackathon.Bff/Integrations/SproutPayroll/SproutPayrollDelegatingHandler.cs`

**Step 1: Create the Sprout API response models and Refit interface**

Create `Hackathon.Bff/Integrations/SproutPayroll/ISproutPayrollClient.cs`:

```csharp
using Refit;

namespace Hackathon.Bff.Integrations.SproutPayroll;

// ── Sprout API response models (match external API shape) ──

public class SproutPayrollResponse
{
    public SproutPagination Pagination { get; set; } = new();
    public List<SproutPayrollEntry> Data { get; set; } = [];
}

public class SproutPagination
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
}

public class SproutPayrollEntry
{
    public SproutEmployeeInfo EmployeeInformation { get; set; } = new();
    public List<SproutAdjustment>? Adjustments { get; set; }
    public SproutGovernmentDeductions GovernmentStatutoryDeductions { get; set; } = new();
    public decimal BasicSalary { get; set; }
    public decimal Tax { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int PayrollYear { get; set; }
    public int PayrollMonth { get; set; }
    public int PayrollPeriod { get; set; }
}

public class SproutEmployeeInfo
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeIdNumber { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string? Department { get; set; }
}

public class SproutAdjustment
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool Taxable { get; set; }
}

public class SproutGovernmentDeductions
{
    public decimal Sssee { get; set; }
    public decimal Ssser { get; set; }
    public decimal Phee { get; set; }
    public decimal Pher { get; set; }
    public decimal Hdmfee { get; set; }
    public decimal Hdmfer { get; set; }
    public decimal HdmfAdditional { get; set; }
}

// ── Refit interface ──

[Headers("Accept: application/json")]
public interface ISproutPayrollClient
{
    [Get("/api/v1/payrolls/entries/{entryId}/summary")]
    Task<SproutPayrollResponse> GetPayrollSummaryAsync(
        int entryId,
        [Query] string search,
        CancellationToken cancellationToken = default);
}
```

**Step 2: Create the DelegatingHandler**

Create `Hackathon.Bff/Integrations/SproutPayroll/SproutPayrollDelegatingHandler.cs`:

```csharp
namespace Hackathon.Bff.Integrations.SproutPayroll;

public class SproutPayrollDelegatingHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public SproutPayrollDelegatingHandler(IConfiguration configuration)
    {
        _apiKey = configuration["SproutPayroll:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: SproutPayroll:ApiKey");
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-api-key", _apiKey);
        request.Headers.Add("x-payroll-PayrollPieDatabase", "Payroll_GC03");
        return base.SendAsync(request, cancellationToken);
    }
}
```

**Step 3: Commit**

```bash
git add Hackathon.Bff/Integrations/SproutPayroll/ISproutPayrollClient.cs Hackathon.Bff/Integrations/SproutPayroll/SproutPayrollDelegatingHandler.cs
git commit -m "feat: add Sprout payroll Refit client and delegating handler"
```

---

### Task 2: Register Refit Client and Add Config

**Files:**
- Modify: `Hackathon.Bff/Program.cs` (after line 156, after existing Refit registration)
- Modify: `Hackathon.Bff/appsettings.Development.json`

**Step 1: Add Sprout payroll config to appsettings**

Add to `Hackathon.Bff/appsettings.Development.json` (at root level alongside existing keys):

```json
"SproutPayroll": {
    "BaseUrl": "https://app-spr-payrollapi-v3-qa.azurewebsites.net",
    "ApiKey": "x7#hx$Yz@Z6!CB5fxrZWXw5Jeo$7eSA219tqiy5Gu29ktsj6X8"
}
```

**Step 2: Register ISproutPayrollClient in Program.cs**

Add these lines after line 156 (after the existing `IApiServiceClient` Refit registration), and add the required `using` statement at the top:

```csharp
// At top of file, add:
using Hackathon.Bff.Integrations.SproutPayroll;

// After line 156 (after .AddHttpMessageHandler<ApiKeyDelegatingHandler>();), add:
builder.Services.AddTransient<SproutPayrollDelegatingHandler>();
var sproutBaseUrl = configurations["SproutPayroll:BaseUrl"]
    ?? throw new InvalidOperationException("Missing config: SproutPayroll:BaseUrl");
builder.Services
    .AddRefitClient<ISproutPayrollClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(sproutBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler<SproutPayrollDelegatingHandler>();
```

**Step 3: Commit**

```bash
git add Hackathon.Bff/Program.cs Hackathon.Bff/appsettings.Development.json
git commit -m "feat: register Sprout payroll Refit client in DI and add config"
```

---

### Task 3: Create BFF Payroll Summary Endpoint

**Files:**
- Create: `Hackathon.Bff/Features/Payroll/SummaryEndpoint.cs`

**Step 1: Create the endpoint with response DTOs and transformation logic**

Create `Hackathon.Bff/Features/Payroll/SummaryEndpoint.cs`:

```csharp
using FastEndpoints;
using Hackathon.Bff.Integrations.SproutPayroll;
using System.Globalization;

namespace Hackathon.Bff.Features.Payroll;

public class PayrollLineItem
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class PayrollSummaryResponse
{
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public decimal Tax { get; set; }
    public List<PayrollLineItem> Deductions { get; set; } = [];
    public List<PayrollLineItem> Earnings { get; set; } = [];
    public string EmployeeName { get; set; } = string.Empty;
    public string PayrollPeriod { get; set; } = string.Empty;
}

public class SummaryEndpoint : EndpointWithoutRequest<PayrollSummaryResponse>
{
    private readonly ISproutPayrollClient _sproutClient;

    public SummaryEndpoint(ISproutPayrollClient sproutClient)
    {
        _sproutClient = sproutClient;
    }

    public override void Configure()
    {
        Get("/api/v1/payroll/summary");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sproutResponse = await _sproutClient.GetPayrollSummaryAsync(
            1002, "LM_Feaure_TestData", ct);

        var entry = sproutResponse.Data.FirstOrDefault();
        if (entry is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var deductions = new List<PayrollLineItem>
        {
            new() { Name = "SSS", Amount = entry.GovernmentStatutoryDeductions.Sssee },
            new() { Name = "PhilHealth", Amount = entry.GovernmentStatutoryDeductions.Phee },
            new() { Name = "Pag-IBIG", Amount = entry.GovernmentStatutoryDeductions.Hdmfee },
            new() { Name = "Withholding Tax", Amount = entry.Tax },
        };

        var earnings = new List<PayrollLineItem>();

        if (entry.Adjustments is not null)
        {
            foreach (var adj in entry.Adjustments)
            {
                if (adj.Amount < 0)
                    deductions.Add(new PayrollLineItem { Name = adj.Name, Amount = Math.Abs(adj.Amount) });
                else if (adj.Amount > 0)
                    earnings.Add(new PayrollLineItem { Name = adj.Name, Amount = adj.Amount });
            }
        }

        var monthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(entry.PayrollMonth);

        await SendOkAsync(new PayrollSummaryResponse
        {
            GrossPay = entry.BasicSalary,
            NetPay = entry.NetAmount,
            Tax = entry.Tax,
            Deductions = deductions,
            Earnings = earnings,
            EmployeeName = $"{entry.EmployeeInformation.FirstName} {entry.EmployeeInformation.LastName}".Trim(),
            PayrollPeriod = $"{monthName} {entry.PayrollYear} - Period {entry.PayrollPeriod}",
        }, cancellation: ct);
    }
}
```

**Step 2: Build to verify compilation**

Run: `dotnet build Hackathon.Bff`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add Hackathon.Bff/Features/Payroll/SummaryEndpoint.cs
git commit -m "feat: add BFF payroll summary endpoint"
```

---

### Task 4: Add Payroll Client Function to Frontend API Layer

**Files:**
- Modify: `Hackathon.Frontend/src/api/runway-v4-client.ts`
- Modify: `Hackathon.Frontend/src/api/runway-v4-types.ts`

**Step 1: Add payroll types**

Add to the bottom of `Hackathon.Frontend/src/api/runway-v4-types.ts`:

```typescript
// ── Payroll types ──

export interface PayrollLineItem {
  name: string
  amount: number
}

export interface PayrollSummary {
  grossPay: number
  netPay: number
  tax: number
  deductions: PayrollLineItem[]
  earnings: PayrollLineItem[]
  employeeName: string
  payrollPeriod: string
}
```

**Step 2: Add payroll fetch function**

Add to the bottom of `Hackathon.Frontend/src/api/runway-v4-client.ts` (before the closing of the file), and add the import:

```typescript
// Add PayrollSummary to the import from runway-v4-types:
import type { PayrollSummary } from './runway-v4-types'

// Add this function:
export async function fetchPayrollSummary(): Promise<PayrollSummary> {
  const { data } = await api.get<PayrollSummary>('/api/v1/payroll/summary')
  return data
}
```

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/api/runway-v4-types.ts Hackathon.Frontend/src/api/runway-v4-client.ts
git commit -m "feat: add payroll summary types and API client function"
```

---

### Task 5: Add Payroll State and Action to Pinia Store

**Files:**
- Modify: `Hackathon.Frontend/src/stores/runway-v4.ts`

**Step 1: Add payroll import**

Add `fetchPayrollSummary` to the import from `runway-v4-client` (line 15-18):

```typescript
import {
  analyzeRunwayV4,
  diagnoseRunwayV4,
  computeScenariosV4,
  fetchPayrollSummary,
} from '../api/runway-v4-client'
```

Add `PayrollSummary` to the import from `runway-v4-types` (line 3-13):

```typescript
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
```

**Step 2: Add payroll state**

Add after line 59 (`const error = ref<string | null>(null)`):

```typescript
  const payroll = ref<PayrollSummary | null>(null)
  const isLoadingPayroll = ref(false)
```

**Step 3: Add fetchPayroll action**

Add after the `restart()` function (before the `return` block at line 296):

```typescript
  async function fetchPayroll() {
    if (payroll.value) return // already fetched
    isLoadingPayroll.value = true
    try {
      payroll.value = await fetchPayrollSummary()
      monthlyIncome.value = payroll.value.netPay
    } catch (e: any) {
      error.value = e.message || 'Failed to load payroll data'
    } finally {
      isLoadingPayroll.value = false
    }
  }
```

**Step 4: Add to return block**

Add `payroll`, `isLoadingPayroll`, and `fetchPayroll` to the return object (around line 298-346).

In the `// State` section, add:
```typescript
    payroll,
    isLoadingPayroll,
```

In the `// Actions` section, add:
```typescript
    fetchPayroll,
```

**Step 5: Reset payroll in restart()**

Add inside the `restart()` function body (around line 258-293), alongside the other resets:

```typescript
    payroll.value = null
    isLoadingPayroll.value = false
```

**Step 6: Commit**

```bash
git add Hackathon.Frontend/src/stores/runway-v4.ts
git commit -m "feat: add payroll state and fetchPayroll action to Pinia store"
```

---

### Task 6: Wire Up PayFeedScreen to Fetch Payroll on Mount

**Files:**
- Modify: `Hackathon.Frontend/src/components/v4/PayFeedScreen.vue`

**Step 1: Add onMounted hook and payroll fetch**

Replace the `<script setup>` block (lines 64-68) with:

```vue
<script setup lang="ts">
import { onMounted } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()

onMounted(() => {
  store.fetchPayroll()
})
</script>
```

**Step 2: Update the hardcoded values in the template to use payroll data**

Replace the static content in the template. The Net Take-Home Pay display (line 18) should use `store.payroll?.netPay`. The Gross Pay (line 25) should use `store.payroll?.grossPay`. The Deductions total (line 29) should compute the sum of deductions. Add a loading state when `store.isLoadingPayroll` is true.

Replace the entire template (lines 1-62) with:

```vue
<template>
  <div class="max-w-md mx-auto p-4 space-y-4">
    <!-- Loading state -->
    <div v-if="store.isLoadingPayroll" class="flex items-center justify-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-green-600"></div>
    </div>

    <!-- Mock Payslip Card -->
    <div v-else class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
      <div class="p-5">
        <div class="flex items-center justify-between mb-4">
          <div>
            <p class="text-xs text-gray-500 uppercase tracking-wide">
              {{ store.payroll?.payrollPeriod ?? 'Payslip' }}
            </p>
            <p class="text-sm text-gray-600 mt-0.5">Sprout Solutions Inc.</p>
          </div>
          <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
            Paid
          </span>
        </div>

        <div class="border-t border-gray-100 pt-4">
          <p class="text-xs text-gray-500 uppercase tracking-wide">Net Take-Home Pay</p>
          <p class="text-3xl font-bold text-gray-900 mt-1">&#8369;{{ formatAmount(store.payroll?.netPay ?? 0) }}</p>
        </div>

        <div class="mt-4 grid grid-cols-2 gap-3 text-sm">
          <div>
            <p class="text-gray-500">Gross Pay</p>
            <p class="font-medium text-gray-800">&#8369;{{ formatAmount(store.payroll?.grossPay ?? 0) }}</p>
          </div>
          <div>
            <p class="text-gray-500">Deductions</p>
            <p class="font-medium text-gray-800">&#8369;{{ formatAmount(totalDeductions) }}</p>
          </div>
        </div>
      </div>

      <!-- Runway Teaser Strip -->
      <div class="bg-gradient-to-r from-green-50 to-emerald-50 border-t border-green-100 px-5 py-4">
        <div class="flex items-center gap-3">
          <div class="flex-shrink-0 w-10 h-10 rounded-full bg-green-100 flex items-center justify-center">
            <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          </div>
          <div class="flex-1">
            <p class="text-sm font-semibold text-green-900">Runway Check</p>
            <p class="text-xs text-green-700">How long can your savings last?</p>
          </div>
          <span class="text-xs font-medium text-green-600 bg-green-100 px-2 py-0.5 rounded-full">New</span>
        </div>
      </div>
    </div>

    <!-- CTA Button -->
    <button
      :disabled="store.isLoadingPayroll"
      class="w-full py-3.5 px-6 bg-green-600 hover:bg-green-700 disabled:opacity-50 text-white font-semibold rounded-xl transition-colors duration-150 text-base flex items-center justify-center gap-2"
      @click="store.goToScreen(2)"
    >
      Check My Runway
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
      </svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()

onMounted(() => {
  store.fetchPayroll()
})

const totalDeductions = computed(() =>
  store.payroll?.deductions.reduce((sum, d) => sum + d.amount, 0) ?? 0
)

function formatAmount(value: number): string {
  return value.toLocaleString('en-PH', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}
</script>
```

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/components/v4/PayFeedScreen.vue
git commit -m "feat: wire PayFeedScreen to fetch payroll on mount and display live data"
```

---

### Task 7: Update PayrollProfile (Screen 2) to Use Live Payroll Data

**Files:**
- Modify: `Hackathon.Frontend/src/components/v4/PayrollProfile.vue`

**Step 1: Replace the entire component with live-data version**

Replace the full content of `PayrollProfile.vue` with:

```vue
<template>
  <div class="max-w-md mx-auto p-4 space-y-5">
    <!-- Header -->
    <div>
      <h1 class="text-2xl font-bold text-gray-900">Your payroll, ready to go</h1>
      <p class="text-sm text-gray-500 mt-1">
        We pre-filled this from your {{ store.payroll?.payrollPeriod ?? '' }} payslip. Just confirm your savings below.
      </p>
    </div>

    <!-- Pre-filled Payroll Card -->
    <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
      <div class="px-5 pt-4 pb-2 flex items-center justify-between">
        <h2 class="text-base font-semibold text-gray-900">Payroll Breakdown</h2>
        <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
          Pre-filled by Sprout
        </span>
      </div>

      <div class="px-5 pb-4 space-y-3">
        <!-- Gross -->
        <div class="flex justify-between items-center py-2 border-b border-gray-50">
          <span class="text-sm text-gray-600">Gross Pay</span>
          <span class="text-sm font-semibold text-gray-900">&#8369;{{ formatAmount(store.payroll?.grossPay ?? 0) }}</span>
        </div>

        <!-- Earnings (positive adjustments) -->
        <div v-if="store.payroll?.earnings?.length" class="space-y-2 pl-3 border-l-2 border-green-100">
          <div v-for="item in store.payroll.earnings" :key="item.name" class="flex justify-between items-center">
            <span class="text-sm text-gray-500">{{ item.name }}</span>
            <span class="text-sm text-green-600">+&#8369;{{ formatAmount(item.amount) }}</span>
          </div>
        </div>

        <!-- Deductions -->
        <div class="space-y-2 pl-3 border-l-2 border-gray-100">
          <div v-for="item in store.payroll?.deductions ?? []" :key="item.name" class="flex justify-between items-center">
            <span class="text-sm text-gray-500">{{ item.name }}</span>
            <span class="text-sm text-red-600">-&#8369;{{ formatAmount(item.amount) }}</span>
          </div>
        </div>

        <!-- Net Take-Home -->
        <div class="flex justify-between items-center pt-3 border-t border-gray-200">
          <span class="text-base font-semibold text-gray-900">Net Take-Home</span>
          <span class="text-xl font-bold text-green-700">&#8369;{{ formatAmount(store.payroll?.netPay ?? 0) }}</span>
        </div>
      </div>
    </div>

    <!-- Consent Block -->
    <div class="bg-blue-50 border border-blue-100 rounded-xl px-4 py-3">
      <div class="flex gap-2.5">
        <svg class="w-5 h-5 text-blue-500 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
        </svg>
        <div>
          <p class="text-sm font-medium text-blue-900">Your data stays within Sprout</p>
          <p class="text-xs text-blue-700 mt-0.5">
            Payroll data is only used to calculate your personal runway. It is never shared externally.
          </p>
        </div>
      </div>
    </div>

    <!-- Savings Input -->
    <div class="space-y-2">
      <label class="block text-sm font-medium text-gray-700">
        How much do you have in savings right now?
      </label>
      <div class="relative">
        <span class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 font-medium text-sm">&#8369;</span>
        <input
          v-model.number="store.liquidSavings"
          type="number"
          class="w-full pl-8 pr-4 py-3 border border-gray-300 rounded-xl text-base font-medium text-gray-900 focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-shadow"
          placeholder="0"
        />
      </div>
      <p class="text-xs text-gray-400">Include bank accounts, digital wallets, and emergency funds.</p>
    </div>

    <!-- CTA Button -->
    <button
      class="w-full py-3.5 px-6 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-xl transition-colors duration-150 text-base flex items-center justify-center gap-2"
      @click="store.goToScreen(3)"
    >
      Continue
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
      </svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()

function formatAmount(value: number): string {
  return value.toLocaleString('en-PH', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/components/v4/PayrollProfile.vue
git commit -m "feat: replace hardcoded PayrollProfile with live Sprout payroll data"
```

---

### Task 8: Build and Verify

**Step 1: Build the BFF project**

Run: `dotnet build Hackathon.Bff`
Expected: Build succeeded, 0 errors

**Step 2: Build the full solution**

Run: `dotnet build Hackathon.slnx`
Expected: Build succeeded

**Step 3: Commit all remaining changes (if any)**

```bash
git add -A
git commit -m "chore: finalize payroll integration"
```
