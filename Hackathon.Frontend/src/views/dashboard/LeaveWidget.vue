<template>
  <spr-card :show-header="false" :has-content-padding="false">
    <template #content>
      <!-- Header with stats -->
      <div class="px-4 pt-3 pb-0">
        <div class="flex items-center gap-4 mb-3">
          <span class="spr-body-sm-semibold spr-text-color-strong">Leave Credits</span>
          <spr-badge text="8" variant="danger" size="small" />
          <span class="spr-body-sm-semibold spr-text-color-strong ml-4">As employee</span>
          <spr-badge text="8" variant="danger" size="small" />
          <span class="spr-body-sm-semibold spr-text-color-strong ml-4">active requests</span>
          <spr-badge text="8" variant="danger" size="small" />
        </div>

        <!-- Tabs -->
        <spr-tabs :list="tabList" underlined :active-tab="activeTabLabel" @tabIndex="onTabChange" />
      </div>

      <!-- AVAILABLE CREDITS header -->
      <div class="px-4 pt-3 pb-2">
        <span class="spr-body-sm-semibold spr-text-color-supporting tracking-wide">AVAILABLE CREDITS</span>
      </div>

      <!-- Leave type rows -->
      <div class="divide-y spr-border-color-base max-h-[260px] overflow-y-auto">
        <div
          v-for="leave in leaveTypes"
          :key="leave.name"
          class="flex items-center justify-between px-4 py-2"
        >
          <span class="spr-body-sm-regular spr-text-color-supporting">{{ leave.name }}</span>
          <div class="flex items-center gap-1">
            <span class="text-xl font-medium spr-text-kangkong-600">{{ leave.used }}</span>
            <span class="spr-body-sm-regular spr-text-color-supporting">/</span>
            <span class="spr-body-sm-regular spr-text-color-supporting">{{ leave.total }}</span>
            <span class="spr-body-xs-regular spr-text-color-supporting ml-0.5">days</span>
          </div>
        </div>
      </div>
    </template>

    <template #footer>
      <div class="flex items-center justify-between w-full">
        <spr-button variant="tertiary" size="small" hasIcon>
          <Icon icon="ph:eye" />
          <span>View All Requests</span>
        </spr-button>
        <spr-button tone="success" size="small" hasIcon>
          <Icon icon="ph:plus" />
          <span>Apply Request</span>
        </spr-button>
      </div>
    </template>
  </spr-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Icon } from '@iconify/vue'

const tabList = ref([
  { label: 'This Pay Period' },
  { label: 'This Month' },
  { label: 'Year-To-Date' },
])
const activeTabLabel = ref('This Pay Period')

const onTabChange = (index: number) => {
  activeTabLabel.value = tabList.value[index].label
}

const leaveTypes = [
  { name: 'Vacation', used: 20, total: 40 },
  { name: 'Sick', used: 20, total: 40 },
  { name: 'Emergency', used: 20, total: 40 },
  { name: 'Magna Carta', used: 20, total: 40 },
  { name: 'Paternity', used: 20, total: 40 },
  { name: 'Service Incentive', used: 20, total: 40 },
  { name: 'Bereavement', used: 20, total: 40 },
  { name: 'Comprehensive Family Care and Support Leave', used: 20, total: 40 },
]
</script>
