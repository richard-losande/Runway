<template>
  <spr-card title="Payslips" :has-content-padding="false">
    <template #content>
      <!-- Latest Transactions -->
      <div class="px-4 pt-3">
        <span class="spr-body-sm-semibold spr-text-color-supporting">Latest Transactions</span>
      </div>

      <!-- Transaction rows -->
      <div class="divide-y spr-border-color-base px-4">
        <div
          v-for="tx in transactions"
          :key="tx.period"
          class="flex items-center justify-between py-3"
        >
          <div class="flex flex-col">
            <span class="spr-body-sm-semibold spr-text-color-strong">{{ tx.type }}</span>
            <span class="spr-body-sm-regular spr-text-color-supporting">{{ tx.period }}</span>
            <span class="spr-body-sm-regular spr-text-color-supporting">{{ tx.note }}</span>
          </div>
          <div class="flex items-center gap-1">
            <span class="spr-body-sm-semibold spr-text-color-strong">{{ tx.amount }}</span>
            <span class="spr-body-sm-regular spr-text-color-supporting">{{ tx.currency }}</span>
          </div>
        </div>
      </div>
    </template>

    <template #footer>
      <div class="flex flex-col gap-2 w-full">
        <spr-button variant="tertiary" size="small" hasIcon>
          <Icon icon="ph:download-simple" />
          <span>Download BIR 2316</span>
        </spr-button>
        <spr-button variant="tertiary" size="small" hasIcon>
          <Icon icon="ph:eye" />
          <span>View Payroll Summary</span>
        </spr-button>
        <spr-button tone="success" fullwidth>Primary</spr-button>
      </div>
    </template>
  </spr-card>
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
