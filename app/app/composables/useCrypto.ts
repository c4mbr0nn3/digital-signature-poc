/**
 * Cryptographic operations using Web Crypto API
 * Follows PRD specifications for ECDSA signing and AES-GCM decryption
 */

export const useCrypto = () => {
  /**
   * Derives an encryption key from a passphrase using PBKDF2
   * @param passphrase - User's signing passphrase
   * @param salt - Salt bytes from server
   * @returns Derived AES-GCM key
   */
  const deriveKey = async (
    passphrase: string,
    salt: Uint8Array
  ): Promise<CryptoKey> => {
    const encoder = new TextEncoder()
    const passphraseData = encoder.encode(passphrase)

    // Import passphrase as base key
    const baseKey = await window.crypto.subtle.importKey(
      'raw',
      passphraseData,
      'PBKDF2',
      false,
      ['deriveKey']
    )

    // Derive AES-GCM key using PBKDF2 with 600,000 iterations (OWASP 2024)
    const derivedKey = await window.crypto.subtle.deriveKey(
      {
        name: 'PBKDF2',
        salt: salt,
        iterations: 600000,
        hash: 'SHA-256',
      },
      baseKey,
      {
        name: 'AES-GCM',
        length: 256,
      },
      true,
      ['encrypt', 'decrypt']
    )

    return derivedKey
  }

  /**
   * Decrypts the private key using AES-GCM
   * @param encryptedPrivateKey - Encrypted private key bytes
   * @param derivedKey - Key derived from passphrase
   * @param iv - Initialization vector (12 bytes)
   * @returns Decrypted private key as CryptoKey
   */
  const decryptPrivateKey = async (
    encryptedPrivateKey: Uint8Array,
    derivedKey: CryptoKey,
    iv: Uint8Array
  ): Promise<CryptoKey> => {
    // Decrypt using AES-GCM
    const decryptedData = await window.crypto.subtle.decrypt(
      {
        name: 'AES-GCM',
        iv: iv,
      },
      derivedKey,
      encryptedPrivateKey
    )

    // Import decrypted data as ECDSA private key
    const privateKey = await window.crypto.subtle.importKey(
      'pkcs8',
      decryptedData,
      {
        name: 'ECDSA',
        namedCurve: 'P-256',
      },
      true,
      ['sign']
    )

    return privateKey
  }

  /**
   * Computes SHA-256 hash of a string and returns hex representation
   * @param data - String to hash
   * @returns Hex-encoded hash
   */
  const sha256Hash = async (data: string): Promise<string> => {
    const encoder = new TextEncoder()
    const dataBuffer = encoder.encode(data)
    const hashBuffer = await window.crypto.subtle.digest('SHA-256', dataBuffer)

    // Convert to hex string
    const hashArray = Array.from(new Uint8Array(hashBuffer))
    const hashHex = hashArray.map((b) => b.toString(16).padStart(2, '0')).join('')

    return hashHex
  }

  /**
   * Signs data using ECDSA with SHA-256
   * @param data - String to sign
   * @param privateKey - ECDSA private key
   * @returns Signature as Uint8Array
   */
  const signData = async (
    data: string,
    privateKey: CryptoKey
  ): Promise<Uint8Array> => {
    const encoder = new TextEncoder()
    const dataBuffer = encoder.encode(data)

    const signatureBuffer = await window.crypto.subtle.sign(
      {
        name: 'ECDSA',
        hash: { name: 'SHA-256' },
      },
      privateKey,
      dataBuffer
    )

    return new Uint8Array(signatureBuffer)
  }

  /**
   * Converts base64 string to Uint8Array
   * @param base64 - Base64 encoded string
   * @returns Uint8Array
   */
  const base64ToUint8Array = (base64: string): Uint8Array => {
    const binaryString = atob(base64)
    const bytes = new Uint8Array(binaryString.length)
    for (let i = 0; i < binaryString.length; i++) {
      bytes[i] = binaryString.charCodeAt(i)
    }
    return bytes
  }

  /**
   * Converts Uint8Array to base64 string
   * @param bytes - Uint8Array
   * @returns Base64 encoded string
   */
  const uint8ArrayToBase64 = (bytes: Uint8Array): string => {
    let binaryString = ''
    for (let i = 0; i < bytes.length; i++) {
      binaryString += String.fromCharCode(bytes[i])
    }
    return btoa(binaryString)
  }

  /**
   * Generates an ECDSA P-256 key pair
   * @returns Promise with publicKey and privateKey
   */
  const generateKeyPair = async (): Promise<{
    publicKey: CryptoKey
    privateKey: CryptoKey
  }> => {
    const keyPair = await window.crypto.subtle.generateKey(
      {
        name: 'ECDSA',
        namedCurve: 'P-256',
      },
      true, // extractable
      ['sign', 'verify']
    )

    return keyPair
  }

  /**
   * Generates a random 16-byte salt for PBKDF2
   * @returns 16-byte Uint8Array
   */
  const generateSalt = (): Uint8Array => {
    return window.crypto.getRandomValues(new Uint8Array(16))
  }

  /**
   * Generates a random 12-byte IV for AES-GCM
   * @returns 12-byte Uint8Array
   */
  const generateIV = (): Uint8Array => {
    return window.crypto.getRandomValues(new Uint8Array(12))
  }

  /**
   * Encrypts a private key using AES-GCM
   * @param privateKey - ECDSA private key to encrypt
   * @param derivedKey - AES-GCM key derived from passphrase
   * @param iv - Initialization vector (12 bytes)
   * @returns Encrypted private key as Uint8Array
   */
  const encryptPrivateKey = async (
    privateKey: CryptoKey,
    derivedKey: CryptoKey,
    iv: Uint8Array
  ): Promise<Uint8Array> => {
    // Export private key to PKCS8 format
    const privateKeyData = await window.crypto.subtle.exportKey(
      'pkcs8',
      privateKey
    )

    // Encrypt using AES-GCM
    const encryptedData = await window.crypto.subtle.encrypt(
      {
        name: 'AES-GCM',
        iv: iv,
      },
      derivedKey,
      privateKeyData
    )

    return new Uint8Array(encryptedData)
  }

  /**
   * Exports a public key to SPKI format
   * @param publicKey - ECDSA public key to export
   * @returns Public key as Uint8Array
   */
  const exportPublicKey = async (
    publicKey: CryptoKey
  ): Promise<Uint8Array> => {
    const publicKeyData = await window.crypto.subtle.exportKey(
      'spki',
      publicKey
    )

    return new Uint8Array(publicKeyData)
  }

  return {
    deriveKey,
    decryptPrivateKey,
    sha256Hash,
    signData,
    base64ToUint8Array,
    uint8ArrayToBase64,
    generateKeyPair,
    generateSalt,
    generateIV,
    encryptPrivateKey,
    exportPublicKey,
  }
}
