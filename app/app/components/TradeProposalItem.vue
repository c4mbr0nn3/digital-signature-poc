<template>
  <div class="bg-white border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow">
    <div class="flex items-center justify-between">
      <div class="flex items-center gap-6">
        <div>
          <p class="text-sm text-gray-500">Proposal ID</p>
          <p class="text-lg font-semibold text-gray-900">#{{ proposal.id }}</p>
        </div>
        <div>
          <p class="text-sm text-gray-500">Trades</p>
          <p class="text-lg font-semibold text-gray-900">
            {{ proposal.metadata.trades.length }}
          </p>
        </div>
        <div>
          <p class="text-sm text-gray-500 mb-1">Status</p>
          <span
            :class="statusBadgeClass"
            class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
          >
            {{ statusText }}
          </span>
        </div>
      </div>
      <Button
        label="Details"
        variant="primary"
        @click="$emit('info-click')"
      >
        <template #icon>
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </template>
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { TradeProposalDetails } from '~/types/trade'

const props = defineProps<{
  proposal: TradeProposalDetails
}>()

defineEmits<{
  'info-click': []
}>()

const statusText = computed(() => {
  if (props.proposal.status === 'signed') {
    return props.proposal.action === 'accepted' ? 'Accepted' : 'Rejected'
  }
  return 'Pending'
})

const statusBadgeClass = computed(() => {
  if (props.proposal.status === 'signed') {
    return props.proposal.action === 'accepted'
      ? 'bg-green-100 text-green-800'
      : 'bg-red-100 text-red-800'
  }
  return 'bg-gray-100 text-gray-800'
})
</script>
