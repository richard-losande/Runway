<template>
  <spr-card :show-header="false" :show-footer="false" :has-content-padding="false">
    <template #content>
      <!-- Decorative top border -->
      <div class="h-[18px] spr-bg-kangkong-900 relative overflow-hidden">
        <svg class="absolute inset-0 w-full h-full" viewBox="0 0 782 18" preserveAspectRatio="none">
          <path d="M0 18 Q200 -5 400 12 T782 0 V18 Z" fill="#22c558" opacity="0.6" />
          <path d="M0 18 Q150 0 350 15 T782 5 V18 Z" fill="#e0c651" opacity="0.4" />
          <path d="M0 18 Q250 5 500 10 T782 2 V18 Z" fill="#1ed5f2" opacity="0.3" />
        </svg>
      </div>

      <!-- Content -->
      <div class="flex items-center justify-between px-4 py-4 gap-4">
        <div class="flex items-center gap-4">
          <spr-avatar variant="initial" initial="Jane Doe" size="xl" badge :status="'disabled'" />

          <div class="flex flex-col">
            <span class="spr-heading-md spr-text-color-strong">Hello, Jane 👋</span>
            <div class="flex items-center gap-2">
              <span class="spr-body-md-regular spr-text-color-supporting">Expected at 09:00 AM</span>
              <spr-lozenge label="09:00 AM - 06:00 PM" tone="success" icon="ph:clock" />
            </div>
          </div>
        </div>

        <!-- Clock buttons -->
        <div class="flex items-center gap-2">
          <spr-button tone="success" size="medium" hasIcon>
            <Icon icon="ph:sign-in" />
            <span>Clock In</span>
          </spr-button>
          <spr-button variant="secondary" size="medium" hasIcon>
            <Icon icon="ph:sign-out" />
            <span>Clock Out</span>
          </spr-button>
        </div>
      </div>
    </template>
  </spr-card>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'
</script>
