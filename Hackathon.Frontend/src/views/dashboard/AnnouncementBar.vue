<template>
  <div class="flex items-center justify-between px-6 py-4 bg-white border-b border-[#eff1f1]">
    <div class="flex items-center gap-2 min-w-0">
      <Icon icon="ph:megaphone-fill" class="text-[#17ad49] text-xl flex-shrink-0" />
      <span class="text-sm font-medium text-[#00291b] flex-shrink-0">Let's Celebrate, Happy Birthday!</span>
      <p class="text-sm text-[#3c5b51] truncate">
        Happy Birthday to these Rockstars! We hope your special day is filled with joy, laughter, and everything that makes you smile.
      </p>
    </div>
    <button class="flex items-center gap-1 bg-[#eff1f1] rounded-md px-1 py-1.5 text-xs font-medium text-[#262b2b] flex-shrink-0">
      <span class="px-1">see more</span>
      <Icon icon="ph:caret-right" class="text-xs" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'
</script>
