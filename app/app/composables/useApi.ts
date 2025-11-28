export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBaseUrl || 'http://localhost:5071/api/v1'

  const apiFetch = async <T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<{ data: T | null; error: string | null }> => {
    try {
      const response = await fetch(`${baseURL}${endpoint}`, {
        headers: {
          'Content-Type': 'application/json',
          ...options.headers,
        },
        ...options,
      })

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`)
      }

      const data = await response.json()
      return { data, error: null }
    } catch (error) {
      console.error('API Error:', error)
      return {
        data: null,
        error: error instanceof Error ? error.message : 'Unknown error',
      }
    }
  }

  return { apiFetch, baseURL }
}
