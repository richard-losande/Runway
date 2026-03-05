<template>
  <spr-card :show-header="false" :show-footer="false" :has-content-padding="false">
    <template #content>
      <!-- Content -->
      <div class="flex items-center justify-between px-4 py-4 gap-4">
        <div class="flex items-center gap-4">
          <spr-avatar variant="initial" :initial="employeeName" size="xl" badge :status="'disabled'" />

          <div class="flex flex-col">
            <span class="spr-text-color-strong" style="font-size: 24px; font-weight: 600;">Hello, {{ firstName }} 👋</span>
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
import { computed } from 'vue'
import { Icon } from '@iconify/vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()
const employeeName = computed(() => store.payroll?.employeeName || 'Jane Doe')
const firstName = computed(() => employeeName.value.split(' ')[0])
</script>
