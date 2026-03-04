<template>
  <button
    class="w-9 h-9 rounded-lg flex items-center justify-center transition-colors"
    :class="active ? 'spr-bg-kangkong-50' : 'hover:spr-bg-mushroom-50'"
    :title="label"
  >
    <spr-icon :id="label" :icon="icon" size="2xs" :variant="active ? 'secondary' : 'tertiary'" />
  </button>
</template>

<script setup lang="ts">
defineProps<{
  icon: string
  label: string
  active?: boolean
}>()
</script>
