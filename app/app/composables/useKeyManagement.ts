import type { CustomerActiveKeyResponse, CustomerKeyOnboardingRequest } from '~/types/trade'

/**
 * Key management composable
 * Handles key onboarding, rotation, and active key state
 */
export const useKeyManagement = () => {
  const { apiFetch } = useApi()
  const crypto = useCrypto()

  const hasActiveKey = ref<boolean | null>(null) // null = unknown, true/false = known
  const isProcessing = ref(false)
  const error = ref<string | null>(null)
  const successMessage = ref<string | null>(null)

  /**
   * Validates passphrase complexity requirements
   * @param passphrase - Passphrase to validate
   * @returns Array of error messages (empty if valid)
   */
  const validatePassphrase = (passphrase: string): string[] => {
    const errors: string[] = []

    if (passphrase.length < 10) {
      errors.push('Passphrase must be at least 10 characters')
    }

    if (!/[A-Z]/.test(passphrase)) {
      errors.push('Passphrase must contain at least one uppercase letter')
    }

    if (!/[a-z]/.test(passphrase)) {
      errors.push('Passphrase must contain at least one lowercase letter')
    }

    if (!/\d/.test(passphrase)) {
      errors.push('Passphrase must contain at least one digit')
    }

    return errors
  }

  /**
   * Checks if the user has an active signing key
   * Updates hasActiveKey state
   */
  const checkActiveKey = async (): Promise<void> => {
    const { data, error: apiError } = await apiFetch<CustomerActiveKeyResponse>(
      '/users/me/keys/active'
    )

    if (data) {
      hasActiveKey.value = true
    } else if (apiError && apiError.includes('404')) {
      // 404 is expected for users without keys
      hasActiveKey.value = false
    } else {
      // Other errors (network, 500, etc.)
      hasActiveKey.value = null
      error.value = apiError || 'Failed to check active key status'
    }
  }

  /**
   * Onboards a new signing key for the user
   * @param passphrase - User's signing passphrase
   * @param passphraseConfirm - Confirmation of passphrase
   * @returns Success status
   */
  const onboardKey = async (
    passphrase: string,
    passphraseConfirm: string
  ): Promise<boolean> => {
    isProcessing.value = true
    error.value = null
    successMessage.value = null

    try {
      // Validate passphrases match
      if (passphrase !== passphraseConfirm) {
        error.value = 'Passphrases do not match'
        return false
      }

      // Validate passphrase complexity
      const validationErrors = validatePassphrase(passphrase)
      if (validationErrors.length > 0) {
        error.value = validationErrors.join('. ')
        return false
      }

      // Generate key pair
      const { publicKey, privateKey } = await crypto.generateKeyPair()

      // Generate salt and IV
      const salt = crypto.generateSalt()
      const iv = crypto.generateIV()

      // Derive encryption key from passphrase
      const derivedKey = await crypto.deriveKey(passphrase, salt)

      // Encrypt private key
      const encryptedPrivateKey = await crypto.encryptPrivateKey(
        privateKey,
        derivedKey,
        iv
      )

      // Export public key
      const publicKeyBytes = await crypto.exportPublicKey(publicKey)

      // Convert to base64 for API transmission
      const request: CustomerKeyOnboardingRequest = {
        publicKey: crypto.uint8ArrayToBase64(publicKeyBytes),
        encryptedPrivateKey: crypto.uint8ArrayToBase64(encryptedPrivateKey),
        salt: crypto.uint8ArrayToBase64(salt),
        iv: crypto.uint8ArrayToBase64(iv),
      }

      // Submit to server
      const { data, error: apiError } = await apiFetch('/users/me/keys/onboarding', {
        method: 'POST',
        body: JSON.stringify(request),
      })

      if (apiError || !data) {
        error.value = apiError || 'Failed to onboard key'
        return false
      }

      // Success
      hasActiveKey.value = true
      successMessage.value = 'Key onboarded successfully! You can now sign trade proposals.'
      return true
    } catch (err) {
      console.error('Key onboarding error:', err)
      error.value =
        err instanceof Error
          ? err.message
          : 'An error occurred during key onboarding'
      return false
    } finally {
      isProcessing.value = false
    }
  }

  /**
   * Rotates the user's signing key (supersedes old key)
   * @param passphrase - User's NEW signing passphrase
   * @param passphraseConfirm - Confirmation of passphrase
   * @returns Success status
   */
  const rotateKey = async (
    passphrase: string,
    passphraseConfirm: string
  ): Promise<boolean> => {
    isProcessing.value = true
    error.value = null
    successMessage.value = null

    try {
      // Validate passphrases match
      if (passphrase !== passphraseConfirm) {
        error.value = 'Passphrases do not match'
        return false
      }

      // Validate passphrase complexity
      const validationErrors = validatePassphrase(passphrase)
      if (validationErrors.length > 0) {
        error.value = validationErrors.join('. ')
        return false
      }

      // Generate NEW key pair
      const { publicKey, privateKey } = await crypto.generateKeyPair()

      // Generate NEW salt and IV
      const salt = crypto.generateSalt()
      const iv = crypto.generateIV()

      // Derive encryption key from NEW passphrase
      const derivedKey = await crypto.deriveKey(passphrase, salt)

      // Encrypt NEW private key
      const encryptedPrivateKey = await crypto.encryptPrivateKey(
        privateKey,
        derivedKey,
        iv
      )

      // Export NEW public key
      const publicKeyBytes = await crypto.exportPublicKey(publicKey)

      // Convert to base64 for API transmission
      const request: CustomerKeyOnboardingRequest = {
        publicKey: crypto.uint8ArrayToBase64(publicKeyBytes),
        encryptedPrivateKey: crypto.uint8ArrayToBase64(encryptedPrivateKey),
        salt: crypto.uint8ArrayToBase64(salt),
        iv: crypto.uint8ArrayToBase64(iv),
      }

      // Submit to server (rotate endpoint)
      const { data, error: apiError } = await apiFetch('/users/me/keys/rotate', {
        method: 'POST',
        body: JSON.stringify(request),
      })

      if (apiError || !data) {
        error.value = apiError || 'Failed to rotate key'
        return false
      }

      // Success
      hasActiveKey.value = true
      successMessage.value = 'Key rotated successfully. Your old key is superseded.'
      return true
    } catch (err) {
      console.error('Key rotation error:', err)
      error.value =
        err instanceof Error
          ? err.message
          : 'An error occurred during key rotation'
      return false
    } finally {
      isProcessing.value = false
    }
  }

  /**
   * Clears error and success messages
   */
  const clearMessages = () => {
    error.value = null
    successMessage.value = null
  }

  return {
    hasActiveKey: readonly(hasActiveKey),
    isProcessing: readonly(isProcessing),
    error: readonly(error),
    successMessage: readonly(successMessage),
    checkActiveKey,
    onboardKey,
    rotateKey,
    clearMessages,
  }
}
