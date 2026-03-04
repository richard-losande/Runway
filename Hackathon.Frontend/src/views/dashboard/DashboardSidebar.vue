<template>
  <nav class="flex flex-col items-center bg-white border-r spr-border-color-base w-[68px] min-h-screen py-4 px-3 gap-2">
    <!-- Logo -->
    <div class="mb-2">
      <spr-logo name="hr" theme="dark" :width="36" />
    </div>

    <!-- Main nav icons -->
    <SidebarIcon icon="ph:magnifying-glass" label="Search" />
    <SidebarIcon icon="ph:star-four" label="AI" />
    <SidebarIcon icon="ph:house-simple-fill" label="Dashboard" :active="true" />
    <SidebarIcon icon="ph:chart-bar" label="Analytics" />
    <SidebarIcon icon="ph:squares-four" label="Apps" />

    <div class="w-9 h-[2px] spr-bg-mushroom-100 my-1" />

    <SidebarIcon icon="ph:money" label="Payroll" />
    <SidebarIcon icon="ph:wallet" label="Finances" />
    <SidebarIcon icon="ph:flow-arrow" label="Flow" />

    <div class="w-9 h-[2px] spr-bg-mushroom-100 my-1" />

    <SidebarIcon icon="ph:gear" label="Setup" />

    <div class="flex-1" />

    <!-- Bottom icons -->
    <SidebarIcon icon="ph:bell" label="Notifications" />
    <SidebarIcon icon="ph:file-text" label="Requests" />

    <!-- Profile avatar -->
    <div class="mt-2">
      <spr-badge text="16" variant="danger" size="big" position="top">
        <spr-avatar variant="initial" initial="Jane Doe" size="md" badge :status="'disabled'" />
      </spr-badge>
    </div>
  </nav>
</template>

<script setup lang="ts">
import SidebarIcon from './SidebarIcon.vue'
</script>
