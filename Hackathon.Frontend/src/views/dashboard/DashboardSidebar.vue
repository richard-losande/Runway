<template>
  <nav class="flex flex-col items-center bg-white border-r border-[#eff1f1] w-[68px] min-h-screen py-4 px-3 gap-2">
    <!-- Logo -->
    <div class="w-9 h-9 bg-[#00291b] rounded flex items-center justify-center mb-2">
      <span class="text-white text-xs font-bold">S</span>
    </div>

    <!-- Main nav icons -->
    <SidebarIcon icon="ph:magnifying-glass" label="Search" />
    <SidebarIcon icon="ph:star-four" label="AI" />
    <SidebarIcon icon="ph:house-simple-fill" label="Dashboard" :active="true" />
    <SidebarIcon icon="ph:chart-bar" label="Analytics" />
    <SidebarIcon icon="ph:squares-four" label="Apps" />

    <div class="w-9 h-[2px] bg-[#eff1f1] my-1" />

    <SidebarIcon icon="ph:money" label="Payroll" />
    <SidebarIcon icon="ph:wallet" label="Finances" />
    <SidebarIcon icon="ph:flow-arrow" label="Flow" />

    <div class="w-9 h-[2px] bg-[#eff1f1] my-1" />

    <SidebarIcon icon="ph:gear" label="Setup" />

    <div class="flex-1" />

    <!-- Bottom icons -->
    <SidebarIcon icon="ph:bell" label="Notifications" />
    <SidebarIcon icon="ph:file-text" label="Requests" />

    <!-- Profile avatar -->
    <div class="w-9 h-9 rounded-full bg-[#e1e6e4] flex items-center justify-center mt-2 relative">
      <span class="text-[#5a756b] text-xs font-medium">JD</span>
      <span class="absolute -top-1 -right-1 w-6 h-6 bg-[#e14942] rounded-full text-white text-xs flex items-center justify-center font-medium">16</span>
    </div>
  </nav>
</template>

<script setup lang="ts">
import SidebarIcon from './SidebarIcon.vue'
</script>
