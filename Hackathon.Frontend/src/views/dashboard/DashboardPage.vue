<template>
  <div class="flex min-h-screen bg-[#f0f2f2] font-['Rubik']">
    <DashboardSidebar />
    <main class="flex-1 flex flex-col bg-[#f7f8f8]">
      <AnnouncementBar />
      <div class="flex-1 bg-[#f1f2f3] px-20 py-6 flex flex-col gap-6 overflow-y-auto">
        <!-- Page Header -->
        <div class="flex items-center justify-between">
          <h1 class="text-xl font-medium text-black">Dashboard</h1>
          <button class="flex items-center gap-2 bg-white rounded-lg px-2 py-3 text-sm font-medium text-[#262b2b] border border-[#eff1f1]">
            <Icon icon="ph:wrench" class="text-base" />
            Customize Dashboard
          </button>
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
