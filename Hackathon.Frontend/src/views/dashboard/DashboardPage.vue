<template>
  <div class="flex min-h-screen spr-bg-mushroom-50 spr-font-main">
    <DashboardSidebar />
    <main class="flex-1 flex flex-col spr-background-color-surface">
      <AnnouncementBar />
      <div class="flex-1 spr-background-color px-20 py-6 flex flex-col gap-6 overflow-y-auto">
        <!-- Page Header -->
        <div class="flex items-center justify-between">
          <h1 class="spr-heading-md spr-text-color-strong">Dashboard</h1>
          <spr-button variant="secondary" size="small" hasIcon>
            <Icon icon="ph:wrench" />
            <span>Customize Dashboard</span>
          </spr-button>
        </div>

        <ProfileCard />

        <!-- Two-column grid -->
        <div class="grid grid-cols-2 gap-4">
          <div class="flex flex-col gap-4">
            <TasksWidget />
            <LeaveWidget />
          </div>
          <PayslipsWidget />
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'
import DashboardSidebar from './DashboardSidebar.vue'
import AnnouncementBar from './AnnouncementBar.vue'
import ProfileCard from './ProfileCard.vue'
import TasksWidget from './TasksWidget.vue'
import PayslipsWidget from './PayslipsWidget.vue'
import LeaveWidget from './LeaveWidget.vue'
</script>
