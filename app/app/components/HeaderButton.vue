<template>
  <button
    @click="$emit('click')"
    :disabled="disabled"
    :title="title"
    :class="[
      'flex items-center gap-2 px-4 py-2 rounded-lg transition-colors text-sm font-medium',
      'disabled:opacity-50 disabled:cursor-not-allowed',
      colorClass
    ]"
  >
    <!-- Loading spinner -->
    <svg
      v-if="loading"
      class="animate-spin h-5 w-5"
      fill="none"
      viewBox="0 0 24 24"
    >
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>

    <!-- Custom icon slot (if provided and not loading) -->
    <slot v-else-if="$slots.icon" name="icon"></slot>

    <!-- Button text -->
    <span>{{ loading ? loadingText : label }}</span>
  </button>
</template>

<script setup lang="ts">
interface Props {
  label: string
  loadingText?: string
  disabled?: boolean
  loading?: boolean
  title?: string
  variant?: 'primary' | 'success' | 'warning' | 'danger'
}

const props = withDefaults(defineProps<Props>(), {
  loadingText: 'Loading...',
  disabled: false,
  loading: false,
  variant: 'primary'
})

defineEmits<{
  click: []
}>()

const colorClass = computed(() => {
  switch (props.variant) {
    case 'success':
      return 'bg-green-600 text-white hover:bg-green-700'
    case 'warning':
      return 'bg-orange-600 text-white hover:bg-orange-700'
    case 'danger':
      return 'bg-red-600 text-white hover:bg-red-700'
    case 'primary':
    default:
      return 'bg-blue-600 text-white hover:bg-blue-700'
  }
})
</script>
