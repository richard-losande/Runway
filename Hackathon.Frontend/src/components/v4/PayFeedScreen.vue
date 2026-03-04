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
