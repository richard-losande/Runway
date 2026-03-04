<template>
  <spr-card :show-footer="false" :has-content-padding="false">
    <template #header>
      <div class="flex items-center justify-between w-full">
        <div class="flex items-center gap-2">
          <spr-icon id="tasks-icon" icon="ph:check-square-fill" size="2xs" tone="success" variant="tertiary" />
          <span class="spr-body-md-semibold spr-text-color-strong">My Tasks</span>
          <spr-badge text="5" variant="danger" size="big" />
        </div>
        <div class="flex items-center gap-2">
          <spr-button variant="secondary" size="small" hasIcon>
            <Icon icon="ph:arrows-down-up" />
            <span>Sort by</span>
            <Icon icon="ph:caret-down" />
          </spr-button>
          <spr-button variant="secondary" size="small" hasIcon>
            <Icon icon="ph:dots-three-vertical" />
          </spr-button>
        </div>
      </div>
    </template>

    <template #content>
      <div class="divide-y spr-border-color-base">
        <div
          v-for="task in tasks"
          :key="task.title + task.date"
          class="flex items-center justify-between px-4 py-2 hover:spr-bg-mushroom-50 transition-colors"
        >
          <div class="flex items-center gap-2">
            <spr-icon :id="'task-' + task.date" :icon="task.icon" size="2xs" :tone="task.iconTone" variant="secondary" />
            <div class="flex flex-col">
              <span class="spr-body-sm-semibold spr-text-color-strong">{{ task.title }}</span>
              <span class="spr-body-sm-regular spr-text-color-supporting">{{ task.subtitle }}</span>
            </div>
          </div>
          <div class="flex items-center gap-4">
            <span class="spr-body-sm-regular spr-text-color-supporting">{{ task.date }}</span>
            <spr-lozenge :label="task.statusLabel" :tone="task.lozengeTone" :icon="task.icon" />
          </div>
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
    statusLabel: 'Needs resolution',
    lozengeTone: 'caution' as const,
    icon: 'ph:warning-circle',
    iconTone: 'caution' as const,
  },
  {
    title: 'IT Acknowledgement Form',
    subtitle: 'Timothy Chalamat',
    date: 'Jan 03, 2025',
    statusLabel: 'Needs action',
    lozengeTone: 'information' as const,
    icon: 'ph:warning-circle',
    iconTone: 'info' as const,
  },
  {
    title: 'Missing Log',
    subtitle: 'System Generated',
    date: 'Jan 02, 2025',
    statusLabel: 'Needs resolution',
    lozengeTone: 'caution' as const,
    icon: 'ph:warning-circle',
    iconTone: 'caution' as const,
  },
  {
    title: 'Leave Request',
    subtitle: 'Jane Doe',
    date: 'Dec 28, 2024',
    statusLabel: 'Needs action',
    lozengeTone: 'information' as const,
    icon: 'ph:warning-circle',
    iconTone: 'info' as const,
  },
  {
    title: 'Overtime Request',
    subtitle: 'Jane Doe',
    date: 'Dec 20, 2024',
    statusLabel: 'Needs action',
    lozengeTone: 'information' as const,
    icon: 'ph:warning-circle',
    iconTone: 'info' as const,
  },
]
</script>
