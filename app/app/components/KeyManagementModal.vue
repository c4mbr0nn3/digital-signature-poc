<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
    @click.self="handleCancel"
  >
    <div class="bg-white rounded-lg shadow-xl p-6 w-full max-w-md">
      <!-- Dynamic Title -->
      <h3 class="text-xl font-bold text-gray-900 mb-4">
        {{ mode === 'onboard' ? 'Onboard Signing Key' : 'Rotate Signing Key' }}
      </h3>

      <!-- Security Information Banner -->
      <div class="mb-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
        <p class="text-sm text-blue-900 font-medium mb-2">
          Important Security Information:
        </p>
        <ul class="text-sm text-blue-800 space-y-1 list-disc list-inside">
          <li>Your signing passphrase encrypts your private key</li>
          <li>It is NOT the same as your login password</li>
          <li>Never transmitted to the server</li>
          <li v-if="mode === 'rotate'">Your old key will be superseded</li>
        </ul>
      </div>

      <!-- Passphrase Requirements -->
      <div class="mb-4 p-3 bg-gray-50 rounded-lg">
        <p class="text-xs text-gray-700 font-medium mb-1">
          Passphrase Requirements:
        </p>
        <ul class="text-xs text-gray-600 space-y-0.5">
          <li>• Minimum 10 characters</li>
          <li>• At least one uppercase letter</li>
          <li>• At least one lowercase letter</li>
          <li>• At least one digit</li>
        </ul>
      </div>

      <form @submit.prevent="handleSubmit">
        <!-- Passphrase Input -->
        <div class="mb-4">
          <label
            for="passphrase"
            class="block text-sm font-medium text-gray-700 mb-2"
          >
            Signing Passphrase
          </label>
          <input
            id="passphrase"
            v-model="passphrase"
            type="password"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
            :disabled="isProcessing"
            autocomplete="new-password"
            placeholder="Create a strong passphrase"
            required
          />
        </div>

        <!-- Confirm Passphrase Input -->
        <div class="mb-6">
          <label
            for="passphrase-confirm"
            class="block text-sm font-medium text-gray-700 mb-2"
          >
            Confirm Passphrase
          </label>
          <input
            id="passphrase-confirm"
            v-model="passphraseConfirm"
            type="password"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
            :disabled="isProcessing"
            autocomplete="new-password"
            placeholder="Re-enter your passphrase"
            required
          />
        </div>

        <!-- Error Display (within modal) -->
        <div
          v-if="error"
          class="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg"
        >
          <p class="text-sm text-red-800">{{ error }}</p>
        </div>

        <!-- Action Buttons -->
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
            :disabled="isProcessing || !passphrase || !passphraseConfirm || !passphraseMatches"
            class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed font-medium"
          >
            <span v-if="isProcessing">
              {{ mode === 'onboard' ? 'Onboarding...' : 'Rotating...' }}
            </span>
            <span v-else>
              {{ mode === 'onboard' ? 'Onboard Key' : 'Rotate Key' }}
            </span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  isOpen: boolean
  mode: 'onboard' | 'rotate'
  isProcessing?: boolean
  error?: string | null
}

interface Emits {
  (e: 'submit', passphrase: string, passphraseConfirm: string): void
  (e: 'cancel'): void
}

const props = withDefaults(defineProps<Props>(), {
  isProcessing: false,
  error: null,
})

const emit = defineEmits<Emits>()

const passphrase = ref('')
const passphraseConfirm = ref('')

const passphraseMatches = computed(() => {
  return passphrase.value === passphraseConfirm.value
})

// Clear fields when modal closes
watch(
  () => props.isOpen,
  (newValue) => {
    if (!newValue) {
      passphrase.value = ''
      passphraseConfirm.value = ''
    }
  }
)

const handleSubmit = () => {
  if (passphrase.value && passphraseConfirm.value && !props.isProcessing) {
    emit('submit', passphrase.value, passphraseConfirm.value)
  }
}

const handleCancel = () => {
  if (!props.isProcessing) {
    emit('cancel')
  }
}
</script>
