<template>
  <div class="bg-white rounded-2xl overflow-hidden">
    <!-- Header -->
    <div class="flex items-center justify-between px-4 py-3 border-b border-[#eff1f1]">
      <div class="flex items-center gap-2">
        <Icon icon="ph:check-square-fill" class="text-xl text-[#17ad49]" />
        <span class="text-base font-medium text-[#00291b]">My Tasks</span>
        <span class="bg-[#da2f38] text-white text-xs font-medium rounded-full w-5 h-5 flex items-center justify-center">5</span>
      </div>
      <div class="flex items-center gap-2">
        <button class="flex items-center gap-1 bg-white border border-[#eff1f1] rounded-lg px-2 py-1.5 text-xs font-medium text-[#262b2b]">
          <Icon icon="ph:arrows-down-up" class="text-xs" />
          Sort by
          <Icon icon="ph:caret-down" class="text-xs" />
        </button>
        <button class="bg-white border border-[#eff1f1] rounded-lg p-1.5">
          <Icon icon="ph:dots-three-vertical" class="text-sm text-[#262b2b]" />
        </button>
      </div>
    </div>

    <!-- Task rows -->
    <div class="divide-y divide-[#eff1f1]">
      <div
        v-for="task in tasks"
        :key="task.title + task.date"
        class="flex items-center justify-between px-4 py-2 hover:bg-[#f7f8f8] transition-colors"
      >
        <div class="flex items-center gap-2">
          <div class="w-7 h-7 bg-white border border-[#eff1f1] rounded flex items-center justify-center">
            <Icon :icon="task.icon" class="text-sm" :class="task.iconColor" />
          </div>
          <div class="flex flex-col">
            <span class="text-sm font-medium text-[#262b2b]">{{ task.title }}</span>
            <span class="text-sm text-[#5d6c6b]">{{ task.subtitle }}</span>
          </div>
        </div>
        <div class="flex items-center gap-4">
          <span class="text-sm text-[#5d6c6b]">{{ task.date }}</span>
          <span
            class="inline-flex items-center gap-1 rounded-md px-1 py-0.5 text-xs font-medium"
            :class="statusClasses[task.status]"
          >
            <span class="w-2 h-2 rounded-full" :class="statusDotClasses[task.status]" />
            {{ task.statusLabel }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'

const statusClasses: Record<string, string> = {
  resolution: 'bg-[#fff4d3] text-[#cc5c02]',
  action: 'bg-[#d8ebff] text-[#0f6eeb]',
}

const statusDotClasses: Record<string, string> = {
  resolution: 'bg-[#cc5c02]',
  action: 'bg-[#0f6eeb]',
}

const tasks = [
  {
    title: 'Missing Log',
    subtitle: 'System Generated',
    date: 'Jan 06, 2025',
    status: 'resolution',
    statusLabel: 'Needs resolution',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#cc5c02]',
  },
  {
    title: 'IT Acknowledgement Form',
    subtitle: 'Timothy Chalamat',
    date: 'Jan 03, 2025',
    status: 'action',
    statusLabel: 'Needs action',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#0f6eeb]',
  },
  {
    title: 'Missing Log',
    subtitle: 'System Generated',
    date: 'Jan 02, 2025',
    status: 'resolution',
    statusLabel: 'Needs resolution',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#cc5c02]',
  },
  {
    title: 'Leave Request',
    subtitle: 'Jane Doe',
    date: 'Dec 28, 2024',
    status: 'action',
    statusLabel: 'Needs action',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#0f6eeb]',
  },
  {
    title: 'Overtime Request',
    subtitle: 'Jane Doe',
    date: 'Dec 20, 2024',
    status: 'action',
    statusLabel: 'Needs action',
    icon: 'ph:warning-circle',
    iconColor: 'text-[#0f6eeb]',
  },
]
</script>
