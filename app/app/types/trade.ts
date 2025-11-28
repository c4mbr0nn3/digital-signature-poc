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

export interface CustomerActiveKeyResponse {
  id: number
  encryptedPrivateKey: string
  privateKeySalt: string
  iv: string
}

export interface TradeSignRequest {
  tradeId: number
  signature: string
  signedAction: string
  signedAt: number
  signingKeyId: number
}

export interface CustomerKeyOnboardingRequest {
  publicKey: string // Base64 encoded SPKI public key
  encryptedPrivateKey: string // Base64 encoded AES-GCM encrypted PKCS8 private key
  salt: string // Base64 encoded PBKDF2 salt (16 bytes)
  iv: string // Base64 encoded AES-GCM IV (12 bytes)
}
