import type {
  CustomerActiveKeyResponse,
  TradeSignRequest,
  TradeProposalDetails,
} from '~/types/trade'

/**
 * Trade signing workflow composable
 * Implements the complete signing flow as per PRD
 */
export const useSigning = () => {
  const { apiFetch } = useApi()
  const crypto = useCrypto()

  const isProcessing = ref(false)
  const error = ref<string | null>(null)

  /**
   * Fetches the active customer key data from the server
   * @returns Customer key data or null on error
   */
  const fetchActiveKey = async (): Promise<CustomerActiveKeyResponse | null> => {
    const { data, error: apiError } = await apiFetch<CustomerActiveKeyResponse>(
      '/users/me/keys/active'
    )

    if (apiError || !data) {
      error.value = apiError || 'Failed to fetch active key'
      return null
    }

    return data
  }

  /**
   * Constructs the canonical string for signing
   * Format: TRADEv1|{trade_id}|{action}|{signed_at}|{metadata_hash}
   * @param tradeId - Trade recommendation ID
   * @param action - 'accept' or 'reject'
   * @param signedAt - Timestamp in milliseconds
   * @param metadataHash - SHA-256 hash of raw metadata string
   * @returns Canonical string
   */
  const buildCanonicalString = (
    tradeId: number,
    action: string,
    signedAt: string,
    metadataHash: string
  ): string => {
    return `TRADEv1|${tradeId}|${action.toLowerCase()}|${signedAt}|${metadataHash}`
  }

  /**
   * Signs a trade action (accept or reject)
   * Follows the complete workflow from PRD section "Signature Workflow"
   * @param proposal - Trade proposal to sign
   * @param action - 'accept' or 'reject'
   * @param passphrase - User's signing passphrase
   * @returns Success status
   */
  const signTrade = async (
    proposal: TradeProposalDetails,
    action: 'accepted' | 'rejected',
    passphrase: string
  ): Promise<boolean> => {
    isProcessing.value = true
    error.value = null

    try {
      // Step 1: Fetch active key data
      const keyData = await fetchActiveKey()
      if (!keyData) {
        return false
      }

      // Step 2: Convert base64 strings to Uint8Arrays
      const encryptedPrivateKey = crypto.base64ToUint8Array(keyData.encryptedPrivateKey)
      const salt = crypto.base64ToUint8Array(keyData.salt)
      const iv = crypto.base64ToUint8Array(keyData.iv)

      // Step 3: Derive encryption key from passphrase and salt
      const derivedKey = await crypto.deriveKey(passphrase, salt)

      // Step 4: Decrypt private key using derived key and IV
      const privateKey = await crypto.decryptPrivateKey(
        encryptedPrivateKey,
        derivedKey,
        iv
      )

      // Step 5: Hash the raw metadata string (NOT the parsed object)
      const metadataHash = await crypto.sha256Hash(proposal.metadataRaw)

      // Step 6: Build canonical string
      const signedAt = Date.now()
      const canonicalString = buildCanonicalString(
        proposal.id,
        action,
        signedAt.toString(),
        metadataHash
      )

      // Step 7: Sign the canonical string
      const signatureBytes = await crypto.signData(canonicalString, privateKey)
      const signature = crypto.uint8ArrayToBase64(signatureBytes)

      // Step 8: Submit signature to server
      const signRequest: TradeSignRequest = {
        tradeId: proposal.id,
        signature,
        signedAction: action,
        signedAt,
        signingKeyId: keyData.id,
      }

      const { data, error: apiError } = await apiFetch(
        `/trades/${proposal.id}/sign`,
        {
          method: 'POST',
          body: JSON.stringify(signRequest),
        }
      )

      if (apiError || !data) {
        error.value = apiError || 'Failed to submit signature'
        return false
      }

      return true
    } catch (err) {
      console.error('Signing error:', err)
      error.value =
        err instanceof Error
          ? err.message
          : 'An error occurred during signing. Please verify your passphrase.'
      return false
    } finally {
      isProcessing.value = false
    }
  }

  return {
    signTrade,
    isProcessing: readonly(isProcessing),
    error: readonly(error),
  }
}
