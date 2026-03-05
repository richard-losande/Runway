<template>
  <spr-card :show-footer="false" :has-content-padding="false">
    <template #header>
      <div class="flex items-center justify-between w-full p-4">
        <div class="flex items-center gap-2">
          <Icon icon="ph:check-circle" style="width:24px;height:24px;color:#158039;" />
          <span class="spr-text-color-strong" style="font-weight: 600;">My Tasks</span>
          <spr-badge text="5" variant="danger" size="big" />
        </div>
        <div class="flex items-center gap-2">
          <spr-button variant="secondary" size="small" hasIcon>
            <Icon icon="ph:arrows-down-up" />
            <span>Sort by</span>
          </spr-button>
          <spr-button variant="secondary" size="small" hasIcon>
            <Icon icon="ph:dots-three-vertical" />
          </spr-button>
        </div>
      </div>
    </template>

    <template #content>
      <div>
        <div
          v-for="task in tasks"
          :key="task.title + task.date"
          class="flex items-center justify-between px-4 py-3 gap-4"
        >
          <div class="flex flex-col min-w-0 flex-1">
            <span class="spr-body-sm-regular-medium spr-text-color-strong truncate max-w-[180px]" style="font-weight: 600;">{{ task.title }}</span>
            <span class="spr-body-sm-regular spr-text-color-supporting">{{ task.subtitle }}</span>
          </div>
          <span class="spr-body-sm-regular spr-text-color-supporting whitespace-nowrap text-center">{{ task.date }}</span>
          <spr-lozenge :label="task.statusLabel" :tone="task.lozengeTone" />
        </div>
      </div>
    </template>
  </spr-card>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'

const tasks = [
  {
    title: 'Missing Log',
    subtitle: 'System Generated',
    date: 'Jan 06, 2025',
    statusLabel: 'NEEDS RESOLUTION',
    lozengeTone: 'caution' as const,
  },
  {
    title: 'IT Acknowledgement Form',
    subtitle: 'Timothy Chalamat',
    date: 'Jan 03, 2025',
    statusLabel: 'NEEDS ACTION',
    lozengeTone: 'information' as const,
  },
  {
    title: 'PM 101 Course Acknowledgement',
    subtitle: 'Alliah Bernadoni',
    date: 'Dec 26, 2024',
    statusLabel: 'NEEDS ACTION',
    lozengeTone: 'information' as const,
  },
  {
    title: 'Missing Log',
    subtitle: 'System Generated',
    date: 'Dec 15, 2024',
    statusLabel: 'NEEDS RESOLUTION',
    lozengeTone: 'caution' as const,
  },
  {
    title: 'Leave Request',
    subtitle: 'Jane Doe',
    date: 'Dec 10, 2024',
    statusLabel: 'NEEDS ACTION',
    lozengeTone: 'information' as const,
  },
]
</script>
