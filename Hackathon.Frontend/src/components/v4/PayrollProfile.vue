<template>
  <div class="flex flex-col min-h-screen max-w-md mx-auto">
    <div class="flex-1 overflow-y-auto p-4 pb-24 space-y-5">
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
        <div class="rounded-xl border border-gray-200 overflow-hidden">
          <div
            v-for="item in store.payroll?.deductions ?? []"
            :key="item.name"
            class="flex items-center justify-between px-4 py-3.5 border-b border-gray-100"
          >
            <span class="text-sm text-gray-600">{{ item.name }}</span>
            <span class="text-sm font-medium text-gray-900">&#8369;{{ formatAmount(item.amount) }}</span>
          </div>
          <!-- Total Deductions row -->
          <div class="flex items-center justify-between px-4 py-3.5">
            <span class="text-sm font-bold text-gray-900">Total Deductions</span>
            <span class="text-sm font-bold text-red-600">&#8369;{{ formatAmount(totalDeductions) }}</span>
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
    <spr-input
      :model-value="store.liquidSavings"
      label="How much do you have in savings right now?"
      placeholder="0"
      :display-helper="true"
      helper-text="Include bank accounts, digital wallets, and emergency funds."
      @update:model-value="store.liquidSavings = Number($event)"
    >
      <template #prefix>&#8369;</template>
    </spr-input>

    </div>

    <!-- Fixed Footer -->
    <div class="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-100 px-4 py-4 flex gap-3 rounded-b-2xl">
      <spr-button variant="secondary" size="large" fullwidth @click="store.goToScreen(1)">
        Back
      </spr-button>
      <spr-button tone="success" size="large" fullwidth @click="store.goToScreen(3)">
        Continue
      </spr-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()

const totalDeductions = computed(() =>
  store.payroll?.deductions.reduce((sum, d) => sum + d.amount, 0) ?? 0
)

function formatAmount(value: number): string {
  return value.toLocaleString('en-PH', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}
</script>
