<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <h1 class="text-3xl font-bold text-gray-900">Trade Proposals</h1>

          <div class="flex items-center gap-4">
            <!-- Left Group: Key Management Buttons -->
            <div class="flex gap-2">
              <button
                @click="handleOnboardKey"
                :disabled="hasActiveKey === true || hasActiveKey === null"
                class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors text-sm font-medium"
                title="Generate your first signing key"
              >
                Onboard Key
              </button>

              <button
                @click="handleRotateKey"
                class="px-4 py-2 bg-orange-600 text-white rounded-lg hover:bg-orange-700 transition-colors text-sm font-medium"
                title="Generate new key (supersedes old key)"
              >
                Rotate Key
              </button>
            </div>

            <!-- Right Group: Generate Button -->
            <button
              @click="generateProposal"
              :disabled="generating"
              class="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              <svg
                v-if="generating"
                class="animate-spin h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <svg
                v-else
                class="w-5 h-5"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
              </svg>
              {{ generating ? 'Generating...' : 'Generate' }}
            </button>
          </div>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <TradeProposalDetailCard
        v-if="selectedProposal"
        :proposal="selectedProposal"
        @close="closeDetail"
        @signed="handleSigned"
      />

      <!-- Success Banner (Key Management) -->
      <div
        v-if="keySuccessMessage"
        class="mb-6 p-4 bg-green-50 border border-green-200 rounded-lg flex items-start justify-between"
      >
        <div class="flex items-start gap-3">
          <svg
            class="w-5 h-5 text-green-600 mt-0.5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <p class="text-green-800">{{ keySuccessMessage }}</p>
        </div>
        <button
          @click="clearKeyMessages"
          class="text-green-600 hover:text-green-800"
          aria-label="Dismiss"
        >
          <svg
            class="w-5 h-5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>

      <!-- Error Banner (Trade Proposals and Key Management) -->
      <div
        v-if="error || keyError"
        class="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg"
      >
        <p class="text-red-800">{{ error || keyError }}</p>
        <button
          v-if="error"
          @click="fetchProposals"
          class="mt-2 text-sm text-red-600 hover:text-red-800 underline"
        >
          Retry
        </button>
      </div>

      <div v-if="loading" class="flex items-center justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>

      <TradeProposalList
        v-else
        :proposals="proposals"
        @select="selectProposal"
      />

      <div
        v-if="!loading && !error && proposals.length === 0"
        class="text-center py-12"
      >
        <p class="text-gray-500">No trade proposals found</p>
      </div>

      <!-- Key Management Modal -->
      <KeyManagementModal
        :is-open="showKeyModal"
        :mode="keyModalMode"
        :is-processing="keyProcessing"
        :error="keyError"
        @submit="handleKeyModalSubmit"
        @cancel="handleKeyModalCancel"
      />
    </main>
  </div>
</template>

<script setup lang="ts">
// Trade proposals composable
const {
  proposals,
  loading,
  error,
  selectedProposal,
  fetchProposals,
  selectProposal,
  closeDetail,
  generating,
  generateProposal
} = useTradeProposals()

// Key management composable
const {
  hasActiveKey,
  isProcessing: keyProcessing,
  error: keyError,
  successMessage: keySuccessMessage,
  checkActiveKey,
  onboardKey,
  rotateKey,
  clearMessages: clearKeyMessages
} = useKeyManagement()

// Modal state
const showKeyModal = ref(false)
const keyModalMode = ref<'onboard' | 'rotate'>('onboard')

// Key management handlers
const handleOnboardKey = () => {
  keyModalMode.value = 'onboard'
  showKeyModal.value = true
}

const handleRotateKey = () => {
  keyModalMode.value = 'rotate'
  showKeyModal.value = true
}

const handleKeyModalSubmit = async (passphrase: string, passphraseConfirm: string) => {
  const success = keyModalMode.value === 'onboard'
    ? await onboardKey(passphrase, passphraseConfirm)
    : await rotateKey(passphrase, passphraseConfirm)

  if (success) {
    showKeyModal.value = false
  }
  // If error, modal stays open for user to correct
}

const handleKeyModalCancel = () => {
  showKeyModal.value = false
  clearKeyMessages()
}

// Trade signing handler
const handleSigned = async () => {
  // Refresh proposals to show updated status
  await fetchProposals()
  // Close the detail card
  closeDetail()
}

onMounted(() => {
  fetchProposals()
  checkActiveKey() // Check if user has active key
})
</script>
