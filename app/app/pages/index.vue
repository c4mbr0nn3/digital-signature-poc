<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <h1 class="text-3xl font-bold text-gray-900">Trade Proposals</h1>
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
    </header>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <TradeProposalDetailCard
        v-if="selectedProposal"
        :proposal="selectedProposal"
        @close="closeDetail"
      />

      <div v-if="error" class="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
        <p class="text-red-800">{{ error }}</p>
        <button
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
    </main>
  </div>
</template>

<script setup lang="ts">
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

onMounted(() => {
  fetchProposals()
})
</script>
