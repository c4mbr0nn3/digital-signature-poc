export interface TradeProposalDetails {
  id: number
  metadata: TradeMetadata
  metadataRaw: string
  status: TradeStatus
  action: TradeAction
  createdAt: number
  signedAt: number | null
}

export interface TradeMetadata {
  trades: TradeData[]
}

export interface TradeData {
  isin: string
  qty: number
  price: number
  ccy: string
}

export type TradeStatus = 'pending' | 'signed'
export type TradeAction = 'pending' | 'accepted' | 'rejected'

export interface TradeProposalCreateResponse {
  id: number
  metadata: TradeMetadata
  metadataRaw: string
  createdAt: number
}
