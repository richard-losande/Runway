<template>
  <div class="bg-white rounded-2xl overflow-hidden">
    <!-- Header with tabs -->
    <div class="px-4 pt-3 pb-0">
      <div class="flex items-center gap-4 mb-3">
        <span class="text-sm font-medium text-[#262b2b]">Leave Credits</span>
        <span class="bg-[#da2f38] text-white text-xs font-medium rounded-full w-5 h-5 flex items-center justify-center">8</span>
        <span class="text-sm font-medium text-[#262b2b] ml-4">As employee</span>
        <span class="bg-[#da2f38] text-white text-xs font-medium rounded-full w-5 h-5 flex items-center justify-center">8</span>
        <span class="text-sm font-medium text-[#262b2b] ml-4">active requests</span>
        <span class="bg-[#da2f38] text-white text-xs font-medium rounded-full w-5 h-5 flex items-center justify-center">8</span>
      </div>

      <!-- Tabs -->
      <div class="flex gap-1 border-b border-[#eff1f1]">
        <button
          v-for="tab in tabs"
          :key="tab"
          class="px-3 py-2 text-sm transition-colors relative"
          :class="activeTab === tab
            ? 'text-[#158039] font-medium'
            : 'text-[#262b2b]'"
          @click="activeTab = tab"
        >
          {{ tab }}
          <span
            v-if="activeTab === tab"
            class="absolute bottom-0 left-0 right-0 h-0.5 bg-[#158039]"
          />
        </button>
      </div>
    </div>

    <!-- AVAILABLE CREDITS header -->
    <div class="px-4 pt-3 pb-2">
      <span class="text-sm font-medium text-[#738482] tracking-wide">AVAILABLE CREDITS</span>
    </div>

    <!-- Leave type rows -->
    <div class="divide-y divide-[#eff1f1] max-h-[260px] overflow-y-auto">
      <div
        v-for="leave in leaveTypes"
        :key="leave.name"
        class="flex items-center justify-between px-4 py-2"
      >
        <span class="text-sm text-[#5d6c6b]">{{ leave.name }}</span>
        <div class="flex items-center gap-1">
          <span class="text-xl font-medium text-[#158039]">{{ leave.used }}</span>
          <span class="text-sm text-[#738482]">/</span>
          <span class="text-sm text-[#738482]">{{ leave.total }}</span>
          <span class="text-xs text-[#738482] ml-0.5">days</span>
        </div>
      </div>
    </div>

    <!-- Footer actions -->
    <div class="flex items-center justify-between px-4 py-3 border-t border-[#eff1f1]">
      <button class="flex items-center gap-1 text-sm font-medium text-[#262b2b]">
        <Icon icon="ph:eye" class="text-base" />
        View All Requests
      </button>
      <button class="flex items-center gap-1 bg-[#158039] text-white rounded-lg px-3 py-2 text-sm font-medium">
        <Icon icon="ph:plus" class="text-base" />
        Apply Request
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Icon } from '@iconify/vue'

const tabs = ['This Pay Period', 'This Month', 'Year-To-Date']
const activeTab = ref('This Pay Period')

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
