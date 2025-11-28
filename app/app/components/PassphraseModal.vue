<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
    @click.self="handleCancel"
  >
    <div class="bg-white rounded-lg shadow-xl p-6 w-full max-w-md">
      <h3 class="text-xl font-bold text-gray-900 mb-4">
        {{ title }}
      </h3>

      <p class="text-sm text-gray-600 mb-6">
        {{ message }}
      </p>

      <form @submit.prevent="handleSubmit">
        <div class="mb-6">
          <label for="passphrase" class="block text-sm font-medium text-gray-700 mb-2">
            Signing Passphrase
          </label>
          <input
            id="passphrase"
            v-model="passphrase"
            type="password"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
            :disabled="isProcessing"
            autocomplete="off"
            placeholder="Enter your signing passphrase"
            required
          />
        </div>

        <div v-if="error" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg">
          <p class="text-sm text-red-800">{{ error }}</p>
        </div>

        <div class="flex gap-3">
          <button
            type="button"
            @click="handleCancel"
            :disabled="isProcessing"
            class="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="isProcessing || !passphrase"
            class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed font-medium"
          >
            {{ isProcessing ? 'Processing...' : 'Confirm' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  isOpen: boolean
  title: string
  message: string
  isProcessing?: boolean
  error?: string | null
}>()

const emit = defineEmits<{
  submit: [passphrase: string]
  cancel: []
}>()

const passphrase = ref('')

// Clear passphrase when modal closes
watch(
  () => props.isOpen,
  (newValue) => {
    if (!newValue) {
      passphrase.value = ''
    }
  }
)

const handleSubmit = () => {
  if (passphrase.value && !props.isProcessing) {
    emit('submit', passphrase.value)
  }
}

const handleCancel = () => {
  if (!props.isProcessing) {
    emit('cancel')
  }
}
</script>
