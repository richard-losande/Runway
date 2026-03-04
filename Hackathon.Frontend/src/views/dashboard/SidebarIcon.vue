<template>
  <button
    class="w-9 h-9 rounded-lg flex items-center justify-center transition-colors"
    :class="active ? 'bg-[#dcfce6]' : 'hover:bg-[#f7f8f8]'"
    :title="label"
  >
    <Icon :icon="icon" class="text-xl text-[#262b2b]" />
  </button>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue'

defineProps<{
  icon: string
  label: string
  active?: boolean
}>()
</script>
