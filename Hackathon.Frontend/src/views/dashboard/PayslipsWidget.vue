<template>
  <div class="bg-white rounded-2xl overflow-hidden">
    <!-- Header -->
    <div class="flex items-center justify-between px-4 py-3 border-b border-[#eff1f1]">
      <span class="text-base font-medium text-[#00291b]">Payslips</span>
    </div>

    <!-- Latest Transactions -->
    <div class="px-4 pt-3">
      <span class="text-sm font-medium text-[#738482]">Latest Transactions</span>
    </div>

    <!-- Transaction rows -->
    <div class="divide-y divide-[#eff1f1] px-4">
      <div
        v-for="tx in transactions"
        :key="tx.period"
        class="flex items-center justify-between py-3"
      >
        <div class="flex flex-col">
          <div class="flex items-center gap-2">
            <span class="text-sm font-medium text-[#262b2b]">{{ tx.type }}</span>
          </div>
          <span class="text-sm text-[#5d6c6b]">{{ tx.period }}</span>
          <span class="text-sm text-[#5d6c6b]">{{ tx.note }}</span>
        </div>
        <div class="flex items-center gap-1">
          <span class="text-sm font-medium text-[#262b2b]">{{ tx.amount }}</span>
          <span class="text-sm text-[#738482]">{{ tx.currency }}</span>
        </div>
      </div>
    </div>

    <!-- Footer actions -->
    <div class="flex flex-col gap-2 px-4 pb-4 pt-2">
      <button class="flex items-center gap-1 text-sm font-medium text-[#262b2b]">
        <Icon icon="ph:download-simple" class="text-base" />
        Download BIR 2316
      </button>
      <button class="flex items-center gap-1 text-sm font-medium text-[#262b2b]">
        <Icon icon="ph:eye" class="text-base" />
        View Payroll Summary
      </button>
      <button class="bg-[#158039] text-white rounded-lg px-3 py-2 text-sm font-medium w-full mt-1">
        Primary
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'

const transactions = [
  {
    type: 'Payroll',
    period: 'Dec 16 - 31, 2025',
    note: 'Late & Absence',
    amount: '******',
    currency: 'PHP',
  },
  {
    type: 'Payroll',
    period: 'Dec 01 - 15, 2025',
    note: 'Late & Absence',
    amount: '******',
    currency: 'PHP',
  },
  {
    type: '13th Month Pay',
    period: '2024',
    note: 'Late & Absence',
    amount: '******',
    currency: 'PHP',
  },
]
</script>
