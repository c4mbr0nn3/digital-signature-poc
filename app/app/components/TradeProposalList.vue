<template>
  <div class="space-y-4">
    <TradeProposalItem
      v-for="proposal in proposals"
      :key="proposal.id"
      :proposal="proposal"
      @info-click="$emit('select', proposal.id)"
    />
  </div>
</template>

<script setup lang="ts">
import type { TradeProposalDetails } from '~/types/trade'

defineProps<{
  proposals: TradeProposalDetails[]
}>()

defineEmits<{
  select: [id: number]
}>()
</script>
