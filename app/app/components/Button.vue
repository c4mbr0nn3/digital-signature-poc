<template>
  <button
    @click="$emit('click')"
    :disabled="disabled"
    :title="title"
    :class="[
      'flex items-center justify-center gap-2 rounded-lg transition-colors font-medium',
      'disabled:opacity-50 disabled:cursor-not-allowed',
      sizeClass,
      fullWidth ? 'w-full' : '',
      colorClass
    ]"
  >
    <!-- Loading spinner -->
    <svg
      v-if="loading"
      :class="iconSizeClass"
      class="animate-spin"
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
  variant?: 'primary' | 'success' | 'warning' | 'danger' | 'secondary'
  size?: 'sm' | 'md' | 'lg'
  fullWidth?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loadingText: 'Loading...',
  disabled: false,
  loading: false,
  variant: 'primary',
  size: 'md',
  fullWidth: false
})

defineEmits<{
  click: []
}>()

const sizeClass = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'px-3 py-1.5 text-xs'
    case 'lg':
      return 'px-6 py-3 text-base'
    case 'md':
    default:
      return 'px-4 py-2 text-sm'
  }
})

const iconSizeClass = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'h-4 w-4'
    case 'lg':
      return 'h-6 w-6'
    case 'md':
    default:
      return 'h-5 w-5'
  }
})

const colorClass = computed(() => {
  switch (props.variant) {
    case 'success':
      return 'bg-green-600 text-white hover:bg-green-700'
    case 'warning':
      return 'bg-orange-600 text-white hover:bg-orange-700'
    case 'danger':
      return 'bg-red-600 text-white hover:bg-red-700'
    case 'secondary':
      return 'border border-gray-300 text-gray-700 bg-white hover:bg-gray-50'
    case 'primary':
    default:
      return 'bg-blue-600 text-white hover:bg-blue-700'
  }
})
</script>
