<template>
  <div class="bg-white rounded-2xl overflow-hidden">
    <!-- Decorative top border -->
    <div class="h-[18px] bg-[#14532b] relative overflow-hidden">
      <svg class="absolute inset-0 w-full h-full" viewBox="0 0 782 18" preserveAspectRatio="none">
        <path d="M0 18 Q200 -5 400 12 T782 0 V18 Z" fill="#22c558" opacity="0.6" />
        <path d="M0 18 Q150 0 350 15 T782 5 V18 Z" fill="#e0c651" opacity="0.4" />
        <path d="M0 18 Q250 5 500 10 T782 2 V18 Z" fill="#1ed5f2" opacity="0.3" />
      </svg>
    </div>

    <!-- Content -->
    <div class="flex items-center justify-between px-4 py-4 gap-4">
      <div class="flex items-center gap-4">
        <!-- Avatar -->
        <div class="w-14 h-14 rounded-full bg-[#e1e6e4] flex items-center justify-center relative">
          <span class="text-[#5a756b] text-lg font-medium">JD</span>
          <span class="absolute bottom-0 right-0 w-2.5 h-2.5 bg-[#989898] rounded-full border-2 border-white" />
        </div>

        <!-- Info -->
        <div class="flex flex-col">
          <span class="text-xl font-medium text-[#262b2b]">Hello, Jane 👋</span>
          <div class="flex items-center gap-2">
            <span class="text-base text-[#5d6c6b]">Expected at 09:00 AM</span>
            <span class="inline-flex items-center gap-1 bg-[#dcfce6] rounded-md px-1 py-0.5 text-xs font-medium text-[#158039]">
              <Icon icon="ph:clock" class="text-xs" />
              09:00 AM - 06:00 PM
            </span>
          </div>
        </div>
      </div>

      <!-- Clock buttons -->
      <div class="flex items-center gap-2">
        <button class="flex items-center gap-1 bg-[#158039] text-white rounded-lg px-2 py-2.5 text-sm font-medium">
          <Icon icon="ph:sign-in" class="text-base" />
          Clock In
        </button>
        <button class="flex items-center gap-1 bg-[#e6eaea] text-[#262b2b] rounded-lg px-2 py-2.5 text-sm font-medium">
          <Icon icon="ph:sign-out" class="text-base" />
          Clock Out
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'
</script>
