<template>
  <div class="min-h-screen spr-bg-mushroom-50 spr-font-main">
    <DashboardSidebar />
    <main class="flex flex-col min-h-screen" style="margin-left: 64px;">
      <AnnouncementBar />
      <div class="flex-1 px-20 py-6 flex flex-col gap-6">
        <ProfileCard />

        <!-- Two-column grid -->
        <div class="grid grid-cols-2 gap-4 items-start">
          <TasksWidget />
          <PayslipsWidget />
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import DashboardSidebar from './DashboardSidebar.vue'
import AnnouncementBar from './AnnouncementBar.vue'
import ProfileCard from './ProfileCard.vue'
import TasksWidget from './TasksWidget.vue'
import PayslipsWidget from './PayslipsWidget.vue'
</script>
