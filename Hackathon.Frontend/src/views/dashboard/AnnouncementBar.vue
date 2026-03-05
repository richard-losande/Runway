<template>
  <div class="flex items-center justify-between px-6 py-4 border-b spr-border-color-base" style="background: linear-gradient(90deg, #ffffff 0%, #ffffff 50%, #D2F612 100%)">
    <div class="flex items-center gap-2 min-w-0">
      <spr-icon id="announcement-icon" icon="ph:megaphone-fill" size="2xs" tone="success" variant="tertiary" />
      <span class="spr-body-sm-semibold spr-text-color-strong flex-shrink-0">Let's Celebrate, Happy Birthday!</span>
      <p class="spr-body-sm-regular spr-text-color-supporting truncate">
        Happy Birthday to these Rockstars! We hope your special day is filled with joy, laughter, and everything that makes you smile.
      </p>
    </div>
    <spr-button variant="secondary" size="small" hasIcon>
      <span>see more</span>
      <Icon icon="ph:caret-right" />
    </spr-button>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'
</script>
