<template>
  <div class="bg-white border-2 border-blue-500 rounded-lg shadow-lg p-6 mb-6">
    <div class="flex items-center justify-between mb-6">
      <h2 class="text-2xl font-bold text-gray-900">
        Proposal #{{ proposal.id }} Details
      </h2>
      <button
        @click="$emit('close')"
        class="text-gray-400 hover:text-gray-600 transition-colors"
        aria-label="Close"
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>

    <div class="grid grid-cols-3 gap-4 mb-6 pb-6 border-b border-gray-200">
      <div>
        <p class="text-sm text-gray-500">Status</p>
        <span
          :class="statusBadgeClass"
          class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium mt-1"
        >
          {{ statusText }}
        </span>
      </div>
      <div>
        <p class="text-sm text-gray-500">Created At</p>
        <p class="text-sm font-medium text-gray-900 mt-1">
          {{ formatTimestamp(proposal.createdAt) }}
        </p>
      </div>
      <div v-if="proposal.signedAt">
        <p class="text-sm text-gray-500">Signed At</p>
        <p class="text-sm font-medium text-gray-900 mt-1">
          {{ formatTimestamp(proposal.signedAt) }}
        </p>
      </div>
    </div>

    <div class="mb-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">
        Sub-Trades ({{ proposal.metadata.trades.length }})
      </h3>

      <div class="space-y-3">
        <div
          v-for="(trade, index) in proposal.metadata.trades"
          :key="index"
          class="bg-gray-50 border border-gray-200 rounded-lg p-4"
        >
          <div class="grid grid-cols-4 gap-4">
            <div>
              <p class="text-xs text-gray-500 mb-1">ISIN</p>
              <p class="text-sm font-mono font-medium text-gray-900">{{ trade.isin }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Quantity</p>
              <p class="text-sm font-semibold text-gray-900">{{ formatNumber(trade.qty) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Price</p>
              <p class="text-sm font-semibold text-gray-900">{{ formatPrice(trade.price) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Currency</p>
              <p class="text-sm font-medium text-gray-900">{{ trade.ccy }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="proposal.status === 'pending'" class="flex gap-4">
      <button
        @click="handleAccept"
        :disabled="isProcessing"
        class="flex-1 px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {{ isProcessing && pendingAction === 'accept' ? 'Processing...' : 'Accept' }}
      </button>
      <button
        @click="handleReject"
        :disabled="isProcessing"
        class="flex-1 px-6 py-3 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {{ isProcessing && pendingAction === 'reject' ? 'Processing...' : 'Reject' }}
      </button>
    </div>

    <PassphraseModal
      :is-open="showPassphraseModal"
      :title="modalTitle"
      :message="modalMessage"
      :is-processing="isProcessing"
      :error="signingError"
      @submit="handlePassphraseSubmit"
      @cancel="handlePassphraseCancel"
    />
  </div>
</template>

<script setup lang="ts">
import type { TradeProposalDetails } from '~/types/trade'

const props = defineProps<{
  proposal: TradeProposalDetails
}>()

const emit = defineEmits<{
  close: []
  signed: []
}>()

const { formatTimestamp } = useTradeProposals()
const { signTrade, isProcessing, error: signingError } = useSigning()

const showPassphraseModal = ref(false)
const pendingAction = ref<'accept' | 'reject' | null>(null)

const modalTitle = computed(() => {
  return pendingAction.value === 'accept'
    ? 'Accept Trade Proposal'
    : 'Reject Trade Proposal'
})

const modalMessage = computed(() => {
  const action = pendingAction.value === 'accept' ? 'accept' : 'reject'
  return `Enter your signing passphrase to ${action} this trade proposal. This action will be cryptographically signed and cannot be repudiated.`
})

const handleAccept = () => {
  pendingAction.value = 'accept'
  showPassphraseModal.value = true
}

const handleReject = () => {
  pendingAction.value = 'reject'
  showPassphraseModal.value = true
}

const handlePassphraseSubmit = async (passphrase: string) => {
  if (!pendingAction.value) return

  const success = await signTrade(props.proposal, pendingAction.value, passphrase)

  if (success) {
    showPassphraseModal.value = false
    pendingAction.value = null
    emit('signed')
  }
}

const handlePassphraseCancel = () => {
  showPassphraseModal.value = false
  pendingAction.value = null
}

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

const formatNumber = (num: number) => {
  return num.toLocaleString('en-US')
}

const formatPrice = (price: number) => {
  return price.toLocaleString('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })
}
</script>
