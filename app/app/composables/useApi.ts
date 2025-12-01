export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBaseUrl || 'http://localhost:5071/api/v1'

  const apiFetch = async <T>(
    endpoint: string,
    options: Parameters<typeof $fetch>[1] = {}
  ): Promise<{ data: T | null; error: string | null }> => {
    try {
      const data = await $fetch<T>(endpoint, {
        baseURL,
        ...options,
        headers: {
          'Content-Type': 'application/json',
          ...options.headers,
        },
      })

      return { data, error: null }
    } catch (error: any) {
      console.error('API Error:', error)

      const errorMessage = error?.data?.message
        || error?.message
        || error?.statusMessage
        || 'Unknown error'

      return {
        data: null,
        error: errorMessage,
      }
    }
  }

  return { apiFetch, baseURL }
}