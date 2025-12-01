import type { TradeProposalDetails, TradeProposalCreateResponse } from '~/types/trade'

export const useTradeProposals = () => {
  const { apiFetch } = useApi()

  const proposals = ref<TradeProposalDetails[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const selectedProposalId = ref<number | null>(null)
  const generating = ref(false)

  const fetchProposals = async () => {
    loading.value = true
    error.value = null

    const result = await apiFetch<TradeProposalDetails[]>('/trades')

    if (result.error) {
      error.value = result.error
      proposals.value = []
    } else {
      proposals.value = result.data || []
    }

    loading.value = false
  }

  const selectedProposal = computed(() => {
    if (!selectedProposalId.value) return null
    return proposals.value.find(p => p.id === selectedProposalId.value) || null
  })

  const selectProposal = (id: number | null) => {
    selectedProposalId.value = id
  }

  const closeDetail = () => {
    selectedProposalId.value = null
  }

  const formatTimestamp = (timestamp: number): string => {
    return new Date(timestamp).toLocaleString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  const generateProposal = async () => {
    generating.value = true
    error.value = null

    const result = await apiFetch<TradeProposalCreateResponse>('/trades/proposal', {
      method: 'POST'
    })

    if (result.error) {
      error.value = result.error
    } else {
      // Refresh the proposal list to show the new proposal
      await fetchProposals()
    }

    generating.value = false
  }

  return {
    proposals,
    loading: readonly(loading),
    error: readonly(error),
    selectedProposal,
    fetchProposals,
    selectProposal,
    closeDetail,
    formatTimestamp,
    generating: readonly(generating),
    generateProposal,
  }
}
